using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.Data
{
    public static class EntityObjectExtensions
    {
        public static void AttachModify<T>(this ObjectSet<T> entitySet, T entity)
        where T : EntityObject
        {
            entitySet.Context.ContextOptions.LazyLoadingEnabled = false;
            if (entity.EntityState != EntityState.Unchanged)
            {
                entitySet.Attach(entity);
                entitySet.Context.SetModified<T>(entity);
            }
        }

        public static void SetModified<T>(this ObjectContext c, T e)
        where T : EntityObject
        {
            c.ObjectStateManager.ChangeObjectState(e, EntityState.Modified);
        }

        public static T Within<T>(this T entity, ObjectSet<T> entitySet, IEqualityComparer<T> cmp = null)
           where T : EntityObject
        {
            object originalentity = null;

            if (cmp is IEqualityComparer<T>)
            {
                originalentity = entitySet.SingleOrDefault(cmp, entity);
            }
            else
            {
                EntityKey key = entitySet.Context.CreateEntityKey(entitySet.EntitySet.Name, entity);
                entitySet.Context.TryGetObjectByKey(key, out originalentity);
            }

            if (originalentity is T)
            {
                return originalentity as T;
            }

            entitySet.AddObject(entity);
            return entity;
        }

        public static IEnumerable<T> Within<T>(this IEnumerable<T> list, ObjectSet<T> entitySet, IEqualityComparer<T> cmp = null)
            where T : EntityObject
        {
            foreach (T entity in list)
            {
                yield return entity.Within(entitySet, cmp);
            }
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer, T value)
        {
            return source.FirstOrDefault(item => comparer.Equals(item, value));
        }

        public static T SingleOrDefault<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer, T value)
        {
            return source.SingleOrDefault(item => comparer.Equals(item, value));
        }

       
        private static object GetEntityKeyValue<T>(this ObjectSet<T> objectSet, T o, int index = 0)
           where T : EntityObject
        {
            if (o.EntityKey == null)
            {
                throw new EntityException("Entity key is null");
            }

            return o.EntityKey.EntityKeyValues.Length > 0 ? o.EntityKey.EntityKeyValues[index].Value : null;
        }

        private static EntityKey GetEntityKey<T>(this ObjectSet<T> objectSet, object keyValue)
            where T : EntityObject
        {
            var entitySetName = objectSet.Context.DefaultContainerName + "." + objectSet.EntitySet.Name;
            var keyPropertyName = objectSet.EntitySet.ElementType.KeyMembers[0].ToString();
            var entityKey = new EntityKey(entitySetName, new[] { new EntityKeyMember(keyPropertyName, keyValue) });
            return entityKey;
        }


    }
}
