using ApiGatewayApi.ApiConfigs;
using ApiGatewayApi.Exceptions;
using Microsoft.OpenApi.Models;

namespace Tests.ApiConfigs;

public class PathFragmentTreeTest
{

    private readonly PathSegmentTree _pathSegmentTree;

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
        
        _pathSegmentTree = PathSegmentTree.BuildTree(paths);
    }
    
    [Fact]
    public void GivenBasicPath_WhenResolving_ReturnCorrectPathItem()
    {
        var resolved = _pathSegmentTree.ResolvePath("test/path/1");
        Assert.Equal("1", resolved?.Item.Description);
    }

    [Fact]
    public void GivenNonexistentPath_WhenResolving_ReturnNull()
    {
        var resolved = _pathSegmentTree.ResolvePath("unknown");
        Assert.Null(resolved);
    }

    [Fact]
    public void GivenPathWhichMatchesWildcard_WhenResolving_ReturnCorrectPathItem()
    {
        var resolved = _pathSegmentTree.ResolvePath("wildcard/anything");
        Assert.Equal("3", resolved?.Item.Description);
    }

    [Fact]
    public void GivenPathWhosePrefixIsWildcardPath_WhenResolving_ReturnNull()
    {
        var resolved = _pathSegmentTree.ResolvePath("wildcard/anything/else");
        Assert.Null(resolved);
    }

    [Fact]
    public void GivenPathWhichMatchesPathWithWildcard_WhenResolving_ReturnCorrectPathItem()
    {
        var resolved = _pathSegmentTree.ResolvePath("wildcard/anything/something");
        Assert.Equal("4", resolved?.Item.Description);
    }

    [Fact]
    public void GivenPathWhichMatchesMultiplePaths_WhenResolving_ReturnMostSpecificPathItem()
    {
        var resolved = _pathSegmentTree.ResolvePath("wildcard/specific/something");
        Assert.Equal("5", resolved?.Item.Description);
    }

    [Fact]
    public void GivenAmbiguousPathItems_WhenBuildingPathSegmentTree_ThrowApiConfigException()
    {
        var paths = new OpenApiPaths();
        var item1 = new OpenApiPathItem { Description = "1" };
        var item2 = new OpenApiPathItem { Description = "2" };
        paths.Add("test/{something}/path", item1);
        paths.Add("test/{else}/path", item2);
        
        Assert.Throws<ApiConfigException>(() => PathSegmentTree.BuildTree(paths));
    }
}