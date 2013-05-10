using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq.Expressions;

namespace DevelopmentWithADot.EntityFrameworkFilters
{
	public static class DbContextExtensions
	{
		public static void Filter<TParentEntity, TCollectionEntity>(this DbContext context, String navigationProperty, Expression<Func<TCollectionEntity, Boolean>> filter)
			where TParentEntity : class, new()
			where TCollectionEntity : class
		{
			(context as IObjectContextAdapter).ObjectContext.ObjectMaterialized += delegate(Object sender, ObjectMaterializedEventArgs e)
			{
				if (e.Entity is TParentEntity)
				{
					DbCollectionEntry col = context.Entry(e.Entity).Collection(navigationProperty);
					col.CurrentValue = new FilteredCollection<TCollectionEntity>(null, col, filter);
				}
			};
		}

		public static void Filter<TContext, TParentEntity, TCollectionEntity>(this TContext context, Expression<Func<TContext, IDbSet<TParentEntity>>> path, Expression<Func<TParentEntity, ICollection<TCollectionEntity>>> collection, Expression<Func<TCollectionEntity, Boolean>> filter)
			where TContext : DbContext
			where TParentEntity : class, new()
			where TCollectionEntity : class
		{
			Filter(context, collection, filter);
		}

		public static void Filter<TParentEntity, TCollectionEntity>(this DbContext context, Expression<Func<TParentEntity, ICollection<TCollectionEntity>>> path, Expression<Func<TCollectionEntity, Boolean>> filter)
			where TParentEntity : class, new()
			where TCollectionEntity : class
		{
			String navigationProperty = path.ToString().Split('.')[1];

			Filter<TParentEntity, TCollectionEntity>(context, navigationProperty, filter);
		}
	}
}