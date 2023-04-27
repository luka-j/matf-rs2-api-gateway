using ApiGatewayApi.ApiConfigs;
using ApiGatewayApi.Exceptions;
using Microsoft.OpenApi.Models;

namespace Tests;

public class PathFragmentTreeTest
{

    private readonly PathFragmentTree _pathFragmentTree;

    public PathFragmentTreeTest()
    {
        var paths = new OpenApiPaths();
        var item1 = new OpenApiPathItem { Description = "1" };
        var item2 = new OpenApiPathItem { Description = "2" };
        var item3 = new OpenApiPathItem { Description = "3" };
        var item4 = new OpenApiPathItem { Description = "4" };
        var item5 = new OpenApiPathItem { Description = "5" };
        paths.Add("test/path/1", item1);
        paths.Add("test/path/2", item2);
        paths.Add("wildcard/{item}", item3);
        paths.Add("wildcard/{item}/something", item4);
        paths.Add("wildcard/specific/{thing}", item5);
        
        _pathFragmentTree = PathFragmentTree.BuildTree(paths);
    }
    
    [Fact]
    public void ResolveBasicPath()
    {
        var resolved = _pathFragmentTree.ResolvePath("test/path/1");
        Assert.Equal("1", resolved?.Description);
    }

    [Fact]
    public void DontResolveNonexistentPath()
    {
        var resolved = _pathFragmentTree.ResolvePath("unknown");
        Assert.Null(resolved);
    }

    [Fact]
    public void ResolvePathWithWildcard()
    {
        var resolved = _pathFragmentTree.ResolvePath("wildcard/anything");
        Assert.Equal("3", resolved?.Description);
    }

    [Fact]
    public void DontResolveWildcardPathWithTrailingNonMatch()
    {
        var resolved = _pathFragmentTree.ResolvePath("wildcard/anything/else");
        Assert.Null(resolved);
    }

    [Fact]
    public void ResolveWildcardPathWithTrailingMatch()
    {
        var resolved = _pathFragmentTree.ResolvePath("wildcard/anything/something");
        Assert.Equal("4", resolved?.Description);
    }

    [Fact]
    public void ResolveMoreSpecificWildcardPath()
    {
        var resolved = _pathFragmentTree.ResolvePath("wildcard/specific/something");
        Assert.Equal("5", resolved?.Description);
    }

    [Fact]
    public void DontBuildPathTreeWithAmbiguousPaths()
    {
        var paths = new OpenApiPaths();
        var item1 = new OpenApiPathItem { Description = "1" };
        var item2 = new OpenApiPathItem { Description = "2" };
        paths.Add("test/{something}/path", item1);
        paths.Add("test/{else}/path", item2);
        
        Assert.Throws<ApiConfigException>(() => PathFragmentTree.BuildTree(paths));
    }
}