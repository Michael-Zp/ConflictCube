using Engine.Components;
using System.Collections.Generic;

namespace Engine.ModelView
{
    public interface IViewModelElement : IEnumerable<IViewModelElement>
    {
        Transform Transform { get; }
        Material Material { get; }
    }
}
