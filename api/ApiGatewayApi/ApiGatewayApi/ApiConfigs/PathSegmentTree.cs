using System.Collections.Immutable;
using ApiGatewayApi.Exceptions;
using Microsoft.OpenApi.Models;

namespace ApiGatewayApi.ApiConfigs;

/// <summary>
/// 
/// Represents a tree whose nodes are segments of the path part of the URL. Leaves of the tree
/// contain an instance of OpenApiPathItem, representing the endpoint which matches the path
/// that needs to be taken to reach that leaf.
/// 
/// Expected usage: build a tree using BuildTree(OpenApiPaths) static method. On a returned
/// object, call ResolvePath(string) to find associated OpenApiPathItem efficiently.
/// </summary>
public class PathSegmentTree
{

    // we're treating segments which start and end with a curly brace (e.g. {foobar})
    // as path parameters, i.e. wildcards. we don't particularly care about the name
    // of the path param at this stage, so we'll store just {}, which we'll treat
    // specially, in the tree. Curly braces are invalid characters in the path, so 
    // this is fine.
    private static readonly string WILDCARD = "{}";
    
    private readonly Dictionary<string, PathSegmentTree> _children;
    private OpenApiPathItem? _leaf;


    /// <summary>
    /// Build and return a tree corresponding to the OpenApiPaths spec. Leading slashes in paths are ignored.
    /// </summary>
    /// <param name="oasPaths">Spec containing paths and items for each path.</param>
    /// <returns>An instance of PathSegmentTree which can be used for efficient lookup
    /// of OpenApiPathItem for a given path.</returns>
    /// <exception cref="ApiConfigException">Thrown if passed OpenApiPaths contain ambiguous paths,
    /// e.g. /something/{param1} and /something/{else} are ambiguous (all possible paths match both).
    /// Other combinations are not ambiguous, because even if for some paths there can be multiple
    /// matches, more specific path is resolved (equivalently, not ALL possible paths match).</exception>
    public static PathSegmentTree BuildTree(OpenApiPaths oasPaths)
    {
        var wholeTree = new PathSegmentTree();
        foreach (var (path, item) in oasPaths)
        {
            var trimmedPath = path;
            if (path.StartsWith('/')) trimmedPath = path[1..];
            var fragments = trimmedPath.Split('/');
            var tree = wholeTree;
            for (var i = 0; i < fragments.Length ; i++)
            {
                var fragment = fragments[i];
                if (fragment.StartsWith('{') && fragment.EndsWith('}'))
                {
                    fragment = WILDCARD;
                }
                
                if (tree._children.TryGetValue(fragment, out var child))
                {
                    tree = child;
                }
                else
                {
                    tree._children[fragment] = new PathSegmentTree();
                    tree = tree._children[fragment];
                }
            }
            if (tree._leaf != null)
            {
                throw new ApiConfigException("API path is ambiguous: " + path);
            }

            tree._leaf = item;
        }

        return wholeTree;
    }

    /// <summary>
    /// Resolve OpenApiPathItem for a given URL path.
    /// </summary>
    /// <param name="path">path part of the URL, without query params or fragment.
    /// It's assumed path conforms to URL spec.</param>
    /// <returns>OpenApiPathItem which corresponds to the path, or null if none exist. If multiple
    /// items match, more specific match is returned. Paths are treated as hierarchical and
    /// params (wildcards) are treated as less specific, i.e. if there's "a/{b}" is more specific than
    /// "{a}/b", so path "a/b" will be matched to it. Paths without wildcards (e.g. "a/b") are always
    /// the most specific and will be matched first if they exist.</returns>
    public OpenApiPathItem? ResolvePath(string path)
    {
        var fragments = ImmutableQueue.Create(path.Split('/'));
        return ResolvePathInternal(fragments);
    }

    private OpenApiPathItem? ResolvePathInternal(ImmutableQueue<string> fragments)
    {
        if (fragments.Count() == 0)
        {
            return _leaf;
        }

        string topFragment;
        var newQueue = fragments.Dequeue(out topFragment);
        if (_children.TryGetValue(topFragment, out var value))
        {
            var item = value.ResolvePathInternal(newQueue);
            if (item != null) return item;
        }
        if (_children.TryGetValue(WILDCARD, out value))
        {
            return value.ResolvePathInternal(newQueue);
        }

        return null;
    }

    private PathSegmentTree()
    {
        _children = new Dictionary<string, PathSegmentTree>();
    }
}