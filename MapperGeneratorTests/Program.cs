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



var model = entity.ToModel();
var source = model.ToSource();

Console.WriteLine(source);

//var model = entity.ToModel();