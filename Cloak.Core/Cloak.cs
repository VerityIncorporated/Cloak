using AsmResolver;
using AsmResolver.DotNet;
using AsmResolver.DotNet.Builder;
using Cloak.Core.Processors;
using Cloak.Core.Processors.Impl;
using Cloak.Core.Protections;
using Cloak.Core.Protections.Impl;
using Cloak.Core.Protections.Impl.ControlFlow;

namespace Cloak.Core;

public sealed class Cloak
{
    private readonly List<Processor> _processors =
    [
        new Cloner(),
        new RuntimeRenamer()
    ];

    internal ModuleDefinition Module { get; private set; } = null!;

    internal ModuleDefinition RuntimeModule { get; } = ModuleDefinition.FromFile("Cloak.Runtime.dll");

    internal Dictionary<string, IMemberDescriptor> ClonedMembers { get; } = new();
    internal Dictionary<string, TypeDefinition> ClonedTypes { get; } = new();

    internal Generator Generator { get; } = new();

    public List<Protection> Protections { get; } = [
        new StringEncryption(),
        new ControlFlow(),
        new IntEncryption(),
        new Renaming()
    ];

    public void Protect(string inputFile, string outputDestination)
    {
        // Load the input file
        Module = ModuleDefinition.FromFile(inputFile);
        
        // Execute every preprocessor
        _processors.ForEach(p => p.PreProcess(this));
        
        // Execute every enabled protection
        foreach (var protection in Protections.Where(p => p.Enabled))
        {
            protection.Execute(this);
        }
        
        // Execute every postprocessor
        _processors.ForEach(p => p.PostProcess(this));
        
        // Write the module to the target destination
        Module.Write(outputDestination, new ManagedPEImageBuilder(MetadataBuilderFlags.PreserveAll));
    }
}