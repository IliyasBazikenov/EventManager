using Contracts;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;

namespace Repository.DataShaping
{
    public class DataShaper<T> : IDataShaper<T>
    {
        public PropertyInfo[] Properties { get; set; }

        public DataShaper()
        {
            Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString)
        {
            IEnumerable<PropertyInfo> requiredProperties = GetRequiredProperties(fieldsString);
            return FetchData(entities, requiredProperties);
        }

        public ShapedEntity ShapeData(T entity, string fieldsString)
        {
            IEnumerable<PropertyInfo> requiredProperties = GetRequiredProperties(fieldsString);
            return FetchDataForEntity(entity, requiredProperties);
        }

        private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
        {
            if (string.IsNullOrWhiteSpace(fieldsString))
                return Properties.ToList();

            var requiredProperties = new List<PropertyInfo>();
            var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (string field in fields)
            {
                PropertyInfo property = Properties.FirstOrDefault(pi => pi.Name.Equals(field.Trim(),
                    StringComparison.InvariantCultureIgnoreCase));

                if (property == null)
                    continue;

                requiredProperties.Add(property);
            }

            return requiredProperties;
        }

        private IEnumerable<ShapedEntity> FetchData(IEnumerable<T> entities, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedData = new List<ShapedEntity>();

            foreach (T entity in entities)
            {
                var shapedObject = FetchDataForEntity(entity, requiredProperties);
                shapedData.Add(shapedObject);
            }
            return shapedData;
        }

        private ShapedEntity FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedObject = new ShapedEntity();
            foreach (var property in requiredProperties)
            {
                var objectPropertyValue = property.GetValue(entity);
                shapedObject.Entity.TryAdd(property.Name, objectPropertyValue);
            }

            var objectProperty = GetIdProperty(entity); ;
            shapedObject.Id = (Guid)objectProperty.GetValue(entity);

            return shapedObject;
        }

        private PropertyInfo GetIdProperty(T entity)
        {
            var removeString = "DTO";
            var entityName = entity.GetType().Name;
            var index = entityName.IndexOf(removeString);

            var idProp = index < 0
                ? entityName + "Id"
                : entityName.Remove(index, removeString.Length) + "Id";

            return entity.GetType().GetProperty(idProp);
        }
    }
}
