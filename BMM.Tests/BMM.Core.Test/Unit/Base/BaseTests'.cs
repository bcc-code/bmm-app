namespace BMM.Core.Test.Unit.Base
{
	public abstract class BaseTests<TTestSubject>
		: BMM.Core.Test.Unit.Base.BaseTests
	{
		protected TTestSubject Subject { get; private set; }

		protected abstract TTestSubject CreateTestSubject();

		private protected sealed override void InitializeTestSubject()
		{
			Subject = CreateTestSubject();
		}
	}
}
