// See https://aka.ms/new-console-template for more information

using MapperGeneratorTests;

Console.WriteLine("Hello, World!");
var entity = new MainEntity()
{
    Hint = "Some hint",
    Name = "Name",
    Id = 1,
    InsertTime = DateTime.Now,
    IsEnabled = true
};



entity.ToModel();

//var model = entity.ToModel();