// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Data.Entity.ModelConfiguration.Edm
{
    using System.Collections.Generic;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.ModelConfiguration.Utilities;
    using System.Data.Entity.Utilities;

    internal static class EdmPropertyExtensions
    {
        private const string OrderAnnotation = "Order";
        private const string PreferredNameAnnotation = "PreferredName";
        private const string UnpreferredUniqueNameAnnotation = "UnpreferredUniqueName";
        private const string AllowOverrideAnnotation = "AllowOverride";

        public static void CopyFrom(this EdmProperty column, EdmProperty other)
        {
            DebugCheck.NotNull(column);
            DebugCheck.NotNull(other);

            column.IsFixedLength = other.IsFixedLength;
            column.IsMaxLength = other.IsMaxLength;
            column.IsUnicode = other.IsUnicode;
            column.MaxLength = other.MaxLength;
            column.Precision = other.Precision;
            column.Scale = other.Scale;
        }

        public static EdmProperty Clone(this EdmProperty tableColumn)
        {
            DebugCheck.NotNull(tableColumn);

            var columnMetadata
                = new EdmProperty(tableColumn.Name, tableColumn.TypeUsage)
                      {
                          Nullable = tableColumn.Nullable,
                          StoreGeneratedPattern = tableColumn.StoreGeneratedPattern,
                          IsFixedLength = tableColumn.IsFixedLength,
                          IsMaxLength = tableColumn.IsMaxLength,
                          IsUnicode = tableColumn.IsUnicode,
                          MaxLength = tableColumn.MaxLength,
                          Precision = tableColumn.Precision,
                          Scale = tableColumn.Scale
                      };

            tableColumn.Annotations.Each(a => columnMetadata.Annotations.Add(a));

            return columnMetadata;
        }

        public static int? GetOrder(this EdmProperty tableColumn)
        {
            DebugCheck.NotNull(tableColumn);

            return (int?)tableColumn.Annotations.GetAnnotation(OrderAnnotation);
        }

        public static void SetOrder(this EdmProperty tableColumn, int order)
        {
            DebugCheck.NotNull(tableColumn);

            tableColumn.Annotations.SetAnnotation(OrderAnnotation, order);
        }

        public static string GetPreferredName(this EdmProperty tableColumn)
        {
            DebugCheck.NotNull(tableColumn);

            return (string)tableColumn.Annotations.GetAnnotation(PreferredNameAnnotation);
        }

        public static void SetPreferredName(this EdmProperty tableColumn, string name)
        {
            DebugCheck.NotNull(tableColumn);

            tableColumn.Annotations.SetAnnotation(PreferredNameAnnotation, name);
        }

        public static string GetUnpreferredUniqueName(this EdmProperty tableColumn)
        {
            DebugCheck.NotNull(tableColumn);

            return (string)tableColumn.Annotations.GetAnnotation(UnpreferredUniqueNameAnnotation);
        }

        public static void SetUnpreferredUniqueName(this EdmProperty tableColumn, string name)
        {
            DebugCheck.NotNull(tableColumn);

            tableColumn.Annotations.SetAnnotation(UnpreferredUniqueNameAnnotation, name);
        }

        public static void RemoveStoreGeneratedIdentityPattern(this EdmProperty tableColumn)
        {
            DebugCheck.NotNull(tableColumn);

            if (tableColumn.StoreGeneratedPattern
                == StoreGeneratedPattern.Identity)
            {
                tableColumn.StoreGeneratedPattern = StoreGeneratedPattern.None;
            }
        }

        public static bool GetAllowOverride(this EdmProperty column)
        {
            DebugCheck.NotNull(column);

            return (bool)column.Annotations.GetAnnotation(AllowOverrideAnnotation);
        }

        public static void SetAllowOverride(this EdmProperty column, bool allowOverride)
        {
            DebugCheck.NotNull(column);

            column.Annotations.SetAnnotation(AllowOverrideAnnotation, allowOverride);
        }

        public static StoreGeneratedPattern? GetStoreGeneratedPattern(this EdmProperty property)
        {
            DebugCheck.NotNull(property);

            return
                (StoreGeneratedPattern?)
                property.Annotations.GetAnnotation(XmlConstants.StoreGeneratedPattern);
        }

        public static void SetStoreGeneratedPattern(
            this EdmProperty property, StoreGeneratedPattern storeGeneratedPattern)
        {
            DebugCheck.NotNull(property);

            property.Annotations.SetAnnotation(XmlConstants.StoreGeneratedPattern, storeGeneratedPattern);
        }

        public static object GetConfiguration(this EdmProperty property)
        {
            DebugCheck.NotNull(property);

            return property.Annotations.GetConfiguration();
        }

        public static void SetConfiguration(this EdmProperty property, object configuration)
        {
            DebugCheck.NotNull(property);

            property.Annotations.SetConfiguration(configuration);
        }

        public static List<EdmPropertyPath> ToPropertyPathList(this EdmProperty property)
        {
            return ToPropertyPathList(property, new List<EdmProperty>());
        }

        public static List<EdmPropertyPath> ToPropertyPathList(this EdmProperty property, List<EdmProperty> currentPath)
        {
            var propertyPaths = new List<EdmPropertyPath>();
            IncludePropertyPath(propertyPaths, currentPath, property);
            return propertyPaths;
        }

        private static void IncludePropertyPath(
            List<EdmPropertyPath> propertyPaths, List<EdmProperty> currentPath, EdmProperty property)
        {
            currentPath.Add(property);
            if (property.IsUnderlyingPrimitiveType)
            {
                propertyPaths.Add(new EdmPropertyPath(currentPath));
            }
            else if (property.IsComplexType)
            {
                foreach (var p in property.ComplexType.Properties)
                {
                    IncludePropertyPath(propertyPaths, currentPath, p);
                }
            }
            currentPath.Remove(property);
        }
    }
}
