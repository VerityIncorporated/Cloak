namespace Cloak.Core.Protections.Impl;

public class Renaming() : Protection("Renaming", "Renames all classes and methods (excluding constructors) within the module, where methods have a CilMethodBody")
{
    internal override void Execute(Cloak cloak)
    {
        cloak.Module.GetAllTypes()
            .ToList()
            .ForEach(typeDefinition =>
            {
                typeDefinition.Name = cloak.Generator.GenerateName();
                typeDefinition.Methods
                    .Where(m => m.CilMethodBody is not null && !m.IsConstructor)
                    .ToList()
                    .ForEach(methodDefinition => methodDefinition.Name = cloak.Generator.GenerateName());
            });
    }
}