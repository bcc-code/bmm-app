using System;
using BMM.Core.GuardedActions.Base.Interfaces;

namespace BMM.Core.GuardedActions.Navigation.Interfaces
{
	public interface INavigateToViewModelAction : IGuardedActionWithParameter<Type>
	{
	}
}