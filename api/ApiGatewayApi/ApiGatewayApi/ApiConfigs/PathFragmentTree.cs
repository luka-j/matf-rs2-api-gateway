using System.Collections.Immutable;
using ApiGatewayApi.Exceptions;
using Microsoft.OpenApi.Models;

namespace ApiGatewayApi.ApiConfigs;

public class PathFragmentTree
{

    private static readonly string WILDCARD = "{}";
    
    private Dictionary<string, PathFragmentTree> _children;
    private OpenApiPathItem? _leaf;

    public static PathFragmentTree BuildTree(OpenApiPaths oasPaths)
    {
        var wholeTree = new PathFragmentTree();
        foreach (var (path, item) in oasPaths)
        {
            var fragments = path.Split('/');
            var tree = wholeTree;
            for (var i = 0; i < fragments.Length ; i++)
            {
                var fragment = fragments[i];
                if (fragment.StartsWith('{') && fragment.EndsWith('}'))
                {
                    fragment = WILDCARD;
                }
                
                if (tree._children.ContainsKey(fragment))
                {
                    tree = tree._children[fragment];
                }
                else
                {
                    tree._children[fragment] = new PathFragmentTree();
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

    private PathFragmentTree()
    {
        _children = new Dictionary<string, PathFragmentTree>();
    }
}