namespace MapperGenerator;

public class MapFrom(string Name) : Attribute;

public class MapFrom<T> : Attribute where T : class;

public class MapTo(string Name) : Attribute;

public class MapTo<T> : Attribute where T : class;

public class MapAs(string Name) : Attribute;

public class FromMappingFunction(string functionExpression) : Attribute;
public class ToMappingFunction(string functionExpression) : Attribute;

public class ToMappingIgnoreMapping : Attribute;
public class FromMappingFunctionIgnore : Attribute;