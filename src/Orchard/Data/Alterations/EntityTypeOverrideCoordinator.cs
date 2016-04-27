using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata.Builders;
using Orchard.Environment.ShellBuilders.Models;

namespace Orchard.Data.Alterations {
    public class EntityTypeOverrideCoordinator : IEntityTypeOverrideHandler {
        private readonly ShellBlueprint _shellBlueprint;
        private readonly IList<InlineOverride> _inlineOverrides = new List<InlineOverride>();

        public EntityTypeOverrideCoordinator(ShellBlueprint shellBlueprint) {
            _shellBlueprint = shellBlueprint;
        }
        public void Alter(ModelBuilder modelBuilder) {
            foreach (var recordAssembly in _shellBlueprint.Records.Select(x => x.Type.Assembly).Distinct()) {
                var types = from type in recordAssembly.GetExportedTypes()
                            where !type.GetTypeInfo().IsAbstract
                            let entity = (from interfaceType in type.GetInterfaces()
                                          where
                                              interfaceType.GetTypeInfo().IsGenericType &&
                                              interfaceType.GetGenericTypeDefinition() == typeof(IEntityTypeOverride<>)
                                          select interfaceType.GetGenericArguments()[0]).FirstOrDefault()
                            where entity != null
                            select type;
                foreach(var type in types) {
                    Override(type, modelBuilder);
                }
            }
            ApplyOverrides(modelBuilder);
        }

        private void Override(Type overrideType, ModelBuilder modelBuilder) {
            var overrideMethod = typeof(EntityTypeOverrideCoordinator)
                .GetMethod(nameof(OverrideHelper), BindingFlags.NonPublic | BindingFlags.Instance);
            if (overrideMethod == null)
                return;

            var overrideInterfaces = overrideType.GetInterfaces().Where(x => IsEntityTypeOverrideType(x)).ToList();
            var overrideInstance = Activator.CreateInstance(overrideType);

            foreach (var overrideInterface in overrideInterfaces) {
                var entityType = overrideInterface.GetGenericArguments().First();
                AddOverride(entityType, instance => {
                    overrideMethod.MakeGenericMethod(entityType)
                        .Invoke(this, new[] { instance, overrideInstance, modelBuilder });
                });
            }
        }

        private void AddOverride(Type type, Action<object> action) {
            _inlineOverrides.Add(new InlineOverride(type, action));
        }

        private void ApplyOverrides(ModelBuilder builder) {
            foreach (var inlineOverride in _inlineOverrides) {
                var entityTypeBuilderInstance = EntityTypeBuilder(builder, inlineOverride.Type);
                inlineOverride.Apply(entityTypeBuilderInstance);
            }
        }

        private bool IsEntityTypeOverrideType(Type type) {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IEntityTypeOverride<>) &&
                   type.GetGenericArguments().Length > 0;
        }

        private void OverrideHelper<T>(EntityTypeBuilder<T> builder, IEntityTypeOverride<T> mappingOverride, ModelBuilder modelBuilder) where T : class {
            mappingOverride.Override(builder,modelBuilder);
        }

        private object EntityTypeBuilder(ModelBuilder builder, Type type) {
            var entityMethod =
                typeof(ModelBuilder).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(x => x.Name == "Entity" && x.IsGenericMethod);

            var genericEntityMethod = entityMethod?.MakeGenericMethod(type);
            return genericEntityMethod?.Invoke(builder, null);
        }
    }
    public class InlineOverride {
        public Type Type { get; }
        private readonly Action<object> _action;

        public InlineOverride(Type type, Action<object> action) {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Type = type;
            _action = action;
        }

        public void Apply(object entityTypeBuilder) {
            _action(entityTypeBuilder);
        }
    }
}
