using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using System.Collections.Generic;

public class ProductControllerTests
{
    private readonly ProductController _controller;
    private readonly ApplicationDbContext _context;

    public ProductControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _controller = new ProductController(_context);
    }

    [Fact]
    public void GetProducts_ReturnsOkResult()
    {
        _context.Products.AddRange(new List<Product>
        {
            new Product {Name = "Product 1", Price = 100, OwnerUsername = "testUser" },
            new Product {Name = "Product 2", Price = 200, OwnerUsername = "testUser" }
        });
        _context.SaveChanges();
        var result = _controller.GetProducts();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<Product>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
    }

    [Fact]
    public void AddProduct_ReturnsCreatedAtAction()
    {
        var product = new Product { Id = 1, Name = "Product 1", Price = 100, OwnerUsername = "testUser" };
        var result = _controller.AddProduct(product);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetProducts", createdAtActionResult.ActionName);
        Assert.Equal(product.Id, createdAtActionResult.RouteValues["id"]);
    }

    [Fact]
    public void UpdateProduct_ReturnsNoContent()
    {
        var existingProduct = new Product {Name = "Product 1", Price = 100, OwnerUsername = "testUser" };
        _context.Products.Add(existingProduct);
        _context.SaveChanges();
        var updatedProduct = new Product {Name = "Updated Product", Price = 150, OwnerUsername = "testUser" };
        var result = _controller.UpdateProduct(1, updatedProduct);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal("Updated Product", existingProduct.Name);
        Assert.Equal(150, existingProduct.Price);
    }

    [Fact]
    public void DeleteProduct_ReturnsNoContent()
    {
        var product = new Product {Name = "Product 1", Price = 100, OwnerUsername = "testUser" };
        _context.Products.Add(product);
        _context.SaveChanges();
        var result = _controller.DeleteProduct(1);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(_context.Products.Find(1));
    }
}
