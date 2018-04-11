# EasyODBC

  EasyODBC is a [NuGet library](https://www.nuget.org/packages/EasyODBC) that helps in the development of applications that use the ODBC connection type. This library provides to ODBCConnection extension methods that make it easy to execute querys, inserts, updates, and deletes

## Getting Started

To use the EasyODBC you only need use nuget to download and install.

```
Install-Package EasyODBC
```

## How to use

### Query
 
 This method executes an SQL query and returns a list of the specified type.
In the first parameter is the SQL query and the second (optional) parameter is an anonymous object that will contain the values of named parameters and the third (optional) is the transaction if you need.
Examples:

Without parameters:

```csharp
odbcConnection.Query<City>("Select * From City");
```
With named parameters:

To put the named parameters you need use this syntax ?<ParameterName>.
  
The following is an example with a named parameter named CityName:

```csharp
odbcConnection.Query<City>("Select * From City Where name = ?CityName ", new {CityName = "New York"}) ;
```
Last example when you need to pass a transaction:

```csharp
var odbcTransaction = odbcConnection.BeginTransaction ();
odbcConnection.Query<City>("Select * From City Where name = ?CityName", new {CityName = "New York"}, odbcTransaction ;
```
### QueryFirstOrDefault
 
This method executes a SQL query and returns the first value. If it does found any value then return the default value of the specified type.

In the first parameter the SQL query and in the second (optional) parameter an anonymous object that will contain the named parameter values and the third parameter (optional) if you need the transaction.

Without parameters:
```csharp
Int id = odbcConnection.QueryFirstOrDefault<int>("Select ID From City order by name");
```
With named parameters:

To put the named parameters you need use this syntax ?ParameterName.

The following is an example with a named parameter named CityName :
```csharp

City city = odbcConnection.QueryFirstOrDefault<City>("Select * From City Where name like ?CityName ", new { CityName = "% New%"}) ;
```

Last example when you need to pass a transaction:

```csharp
var odbcTransaction = odbcConnection.BeginTransaction ();
odbcConnection.QueryFirstOrDefault<City>("Select * From City Where name = ?CityName ", new { CityName = "New York"}, odbcTransaction ) ;
```

### FindByID
 
This method fetches an object of the type specified by ID
This method needs two parameters the value of the Id and the second (optional) transaction.

Example:

Find an object of type City with id 55
```csharp
City city = odbcConnection.FindByID <City>(55);
```
FindAll
 
This method fetches all records from the specified type table and returns them as an IEnumerable.
This method only one parameter is optional that is the transaction.
Example:
```csharp
IEnumerable<City> cities = odbcConnection.FindAll<City>();
```

### Execute
 
This method executes a SQL query that does not need a return of someone specific type like Delete, Insert or Update. This method returns the number of rows that have been modified.

In the first parameter the SQL query and in the second (optional) parameter an anonymous object that will contain the named parameter values and the third parameter (optional) if you need the transaction.

Examples:

Without parameters:
```csharp
odbcConnection.Execute<City>("Delete From City where name ="New York") ;
```

With named parameters:

To put the named parameters you need use this syntax ?ParameterName.

The following is an example with a named parameter named CityName :
```csharp
odbcConnection.Execute<City>("Delete From City where name = ?CityName ", new { CityName = "New York"})
```
Last example when you need to pass a transaction:

```
var odbcTransaction = odbcConnection.BeginTransaction ();
odbcConnection.Execute<City>("Select * From City Where name =?CityName ", new { CityName = "New York"}, odbcTransaction ) ;
```
### Insert
 
This method performs an insertion of the object into the database.

This has two parameters, the first is the object to be inserted and the second (optional) is the transaction.

Example:

```
City city = new City {name = "Dallas"}
odbcConnection.Insert(city);
```
### Update
 
This method performs an update of the object in the database.

This has two parameters, the first is the object that will be updated and the second (optional) is the transaction.

Example:

```csharp
City city = odbcConnection.FindByID<City>(55);
city.Name = "Denver"
odbcConnection.Update(city);
```
### Delete
 
This method performs a deletion of the object in the database.

This has two parameters, the first is the object to be deleted and the second (optional) is the transaction.

Example:

```csharp
City city = odbcConnection.FindByID<City>(55);
odbcConnection.Delete(city);
 ```

## Attributes
 
The EasyODBC provides some attributes that are required for the operation of the library. They are Table, Field and ID. How to use:

### Table
 
This attribute is optional and serves to tell you what is database and also tell what the name of it in the database in case if they are different from what is informed in the class. If you do not use this attribute, it will be used the class name

Example:

The table in the database is called Table_Cat but the C# class is called Cat , so the class declaration would look like this:

```csharp
[Table(tableName = "Table_Cat")]
public class Cat
{
// Class code
}
```
In addition, if the database was different from the connection database.

The database of the connection is City_DataBase, but the database that the C# class belongs to is the Cats_DataBase, so the class declaration would look like this:
```csharp
[Table(dataBaseName = "Cats_DataBase")]
public class Cat
{
// Class code
}
```
### ID
 
This attribute indicates which of the fields in the class represents the ID in the table. This attribute is required for the Update, Delete, and FindByID methods to work.

In this attribute are some properties:

**AutoGenered** - Indicates whether the field is self-generated or not, this field is true by default.

**FieldName** - indicates the name of the field in the table in the database if by chance the name is different from the C#.

Examples:

The Cat Table has a field that is the ID:
```csharp
[ID]
public int ID {get; set; }
```
And if that field has another name, for example in the table the name is CatID , so the declaration of the field would look like this:
```csharp
[ID(FieldName = " CatID ")]
public int ID {get; set; }
```
In addition, if the ID field were not self-generated it would look like this:
```csharp
[ID(AutoGenered = false)]
public int ID {get; set; }
```

### Field
 
This attribute is optional it can be used to indicate that field can be inserted, updated, autogered, or whether it is a field in the table in database and we can also indicate the name of the field in the table in the database and this is indicated by the following properties:

**FieldName** - Indicates the field name in the table. If this attribute is not used will be used the property name.

**Insertable** - Indicates whether the insert can use the field, by default is set to true.

**Updatable** - Indicates whether the update can use the field, by default it is set to true.

**AutoGenered** - Indicates if the field is self-generated, so it cannot be used in insert commands, but can be used by update, if you want change this just put the Updatable property to false.

**IsTableField** - Indicates whether the field is a field in the table in the database, this field is set to true by default.

Examples:

The class has a property called name, but in the table it is called cat_name , so the declaration of property looks like this:
```csharp
[Field(FieldName = "cat_name" )]
public string name {get; set; }
```
 
If the field cannot be used by the insert. The definition of the property looks like this:
```csharp
[Field(Insertable = false)]
public string name {get; set; }
 ```
If the field cannot be used by the update. The definition of the property looks like this:

```csharp
[Field(Updatable = false)]
public string name {get; set; }
``` 
Now in the class has a field that saves the date and time that the table is inserted, but informs this is the database itself, so we do not want this property can be inserted or updated by EasyODBC. The declaration of this property looks like this:
```csharp
[Field(AutoGenered = true)]
public DateTime Date {get; set; }
 ```

In the class has a property named NameWithID that returns a string that joins the name with the property ID, this property is not part of the table, so to have no problem when using EasyODBC we need to declare the property this way:
```csharp
[Field (IsTableField = false)]
public string NameWithID
{
get
{
return string.Concat (ID, "-", Name);
}
}
 ```

The class at the end looked like this:
```csharp
using EasyODBC.Attributes;

namespace VisualCache.Modelos
{
    [Table(tableName ="Table_Cat")]
    public class Cat
    {
        [ID(FieldName = "CatID")]
        public int ID { get; set; }

        //[Field(Insertable = false)]
        //public string Name { get; set; }

        [Field(FieldName ="cat_name",Updatable = false)]
        public string Name { get; set; }

        [Field(AutoGenered = true)]
        public DateTime Date { get; set; }

        [Field(IsTableField = false)]
        public string NameWithID
        {
            get
            {
                return string.Concat(ID, " - ", Name);
            }
        }
    }
}
```

## Authors

* **Tarcísio Carvalho** - *Initial work* - [tarcCar](https://github.com/tarcCar)

See also the list of [contributors](https://github.com/tarcCar/EasyODBC/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](https://github.com/tarcCar/EasyODBC/blob/master/LICENSE) file for details


