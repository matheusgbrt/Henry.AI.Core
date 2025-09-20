using Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;
using Henry.AI.Core.Host.CodeUnderstanding.Contracts;
using Henry.AI.Core.Host.CodeUnderstanding.Dtos;
using Henry.AI.Core.Host.CodeUnderstanding.Repositories;
using Rebus.Bus;
using Rebus.Handlers;

namespace Henry.AI.Core.Host.CodeUnderstanding.Handlers;

public class CodeUnderstandingResponseHandler : IHandleMessages<CodeUnderstandingResponse>
{
    private readonly IBus _bus;
    private readonly IClassRepository _classRepository;
    private readonly IConstructorRepository _constructorRepository;
    private readonly IMethodRepository _methodRepository;
    private readonly IPropertyRepository _propertyRepository;

    public CodeUnderstandingResponseHandler(IBus bus, IClassRepository classRepository, IConstructorRepository constructorRepository, IMethodRepository methodRepository, IPropertyRepository propertyRepository)
    {
        _bus = bus;
        _classRepository = classRepository;
        _constructorRepository = constructorRepository;
        _methodRepository = methodRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task Handle(CodeUnderstandingResponse message)
    {
        var constructorMapping = message.Classes.ToDictionary(c => c, c => c.Constructors);
        var methodMapping = message.Classes.ToDictionary(c => c, c => c.Methods);
        var propertyMapping = message.Classes.ToDictionary(c => c, c => c.Properties);
        var classes = message.Classes.Select(c => c.ToClass(message.Namespace)).ToList();
        
        await HandleClasses(classes);
        await HandleConstructors(constructorMapping);
        await HandleMethods(methodMapping);
        await HandleProperties(propertyMapping);
        
        
        await Task.CompletedTask;
    }


    private async Task HandleClasses(List<ClassNode> classes)
    {
        foreach (var classFromList in classes)
        {
            var existantClass = await _classRepository.Find(classFromList);
            if (existantClass == null)
            {
                await _classRepository.Create(classFromList);
            }
            else
            {
                
            }
        }
    }

    private async Task HandleConstructors(Dictionary<TClass,List<TConstructor>> constructorsMapping)
    {
        foreach (var constructorMapping in constructorsMapping)
        {
            var tClass = constructorMapping.Key;
            var constructors = constructorMapping.Value;

            foreach (var constructor in constructors)
            {
                var constructorNode = constructor.ToConstructor(tClass.Name);
                var existantConstructor = await _constructorRepository.Find(constructorNode);
                if (existantConstructor == null)
                {
                    await _constructorRepository.Create(constructorNode);
                }
                else
                {
                    
                }
            }
        }
    }

    private async Task HandleMethods(Dictionary<TClass, List<TMethod>> methodsMapping)
    {
        foreach (var methodMapping in methodsMapping)
        {
            var tClass = methodMapping.Key;
            var methods = methodMapping.Value;
            foreach (var method in methods)
            {
                var methodNode = method.ToMethod(tClass.Name);
                var existantMethod = await _methodRepository.Find(methodNode);
                if (existantMethod == null)
                {
                    await _methodRepository.Create(methodNode);
                }
                else
                {
                    
                }
            }
        }
    }

    private async Task HandleProperties(Dictionary<TClass, List<TProperty>> propertiesMapping)
    {
        foreach (var propertyMapping in propertiesMapping)
        {
            var tClass = propertyMapping.Key;
            var properties = propertyMapping.Value;
            foreach (var property in properties)
            {
                var propertyNode = property.ToProperty(tClass.Name);
                var existantProperty = await _propertyRepository.Find(propertyNode);
                if (existantProperty == null)
                {
                    await _propertyRepository.Create(propertyNode);
                }
                else
                {
                    
                }
            }
        }
    }
}