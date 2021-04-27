using System;
using System.Threading.Tasks;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IMenu
    {
        Func<AppQuery, AppQuery> BottomBar { get; }

        Func<AppQuery, AppQuery> SearchMenuItem { get; }

        Func<AppQuery, AppQuery> ExploreMenuItem { get; }

        Func<AppQuery, AppQuery> MyContentMenuItem { get; }

        Func<AppQuery, AppQuery> LibraryMenuItem { get; }

        Func<AppQuery, AppQuery> ProfileMenuItem { get; }

        Func<AppQuery, AppQuery> GermanProfileMenuItem { get; }

        void OpenSearch(IApp app);

        void OpenExplore(IApp app);

        void OpenMyContent(IApp app);

        void OpenLibrary(IApp app);

        void OpenProfilePage(IApp app);
    }

    public abstract class Menu : IMenu
    {
        public const string LabelSearch = "Search";
        public const string LabelExplore = "Explore";
        public const string LabelMyContent = "My Content";
        public const string LabelLibrary = "Library";
        public const string LabelProfile = "Profile";

        public abstract Func<AppQuery, AppQuery> BottomBar { get; }

        public abstract Func<AppQuery, AppQuery> SearchMenuItem { get; }

        public abstract Func<AppQuery, AppQuery> ExploreMenuItem { get; }

        public abstract Func<AppQuery, AppQuery> MyContentMenuItem { get; }

        public abstract Func<AppQuery, AppQuery> LibraryMenuItem { get; }

        public abstract Func<AppQuery, AppQuery> ProfileMenuItem { get; }

        public abstract Func<AppQuery, AppQuery> GermanProfileMenuItem { get; }

        void OpenMenuItem(IApp app, Func<AppQuery, AppQuery> menuItem)
        {
            app.Tap(menuItem);
        }

        public void OpenSearch(IApp app)
        {
            OpenMenuItem(app, SearchMenuItem);
        }

        public void OpenExplore(IApp app)
        {
            OpenMenuItem(app, ExploreMenuItem);
        }

        public void OpenMyContent(IApp app)
        {
            OpenMenuItem(app, MyContentMenuItem);
        }

        public void OpenLibrary(IApp app)
        {
            OpenMenuItem(app, LibraryMenuItem);
        }

        public void OpenProfilePage(IApp app)
        {
            OpenMenuItem(app, ProfileMenuItem);
        }
    }

    public class AndroidMenu : Menu
    {
        public override Func<AppQuery, AppQuery> BottomBar { get { return c => c.Id("bottom_navigation"); } }

        public override Func<AppQuery, AppQuery> SearchMenuItem { get { return c => BottomBar(c).Descendant().Marked(LabelSearch); } }

        public override Func<AppQuery, AppQuery> ExploreMenuItem { get { return c => BottomBar(c).Descendant().Marked(LabelExplore); } }

        public override Func<AppQuery, AppQuery> MyContentMenuItem { get { return c => BottomBar(c).Descendant().Marked(LabelMyContent); } }

        public override Func<AppQuery, AppQuery> LibraryMenuItem { get { return c => BottomBar(c).Descendant().Marked(LabelLibrary); } }

        public override Func<AppQuery, AppQuery> ProfileMenuItem { get { return c => BottomBar(c).Descendant().Marked(LabelProfile); } }

        public override Func<AppQuery, AppQuery> GermanProfileMenuItem { get { return c => BottomBar(c).Descendant().Marked("Profil"); } }
    }

    public class TouchMenu : Menu
    {
        public override Func<AppQuery, AppQuery> BottomBar { get { return c => c.Id("tab_bar"); } }

        public override Func<AppQuery, AppQuery> SearchMenuItem { get { return c => BottomBar(c).Descendant().Text(LabelSearch); } }

        public override Func<AppQuery, AppQuery> ExploreMenuItem { get { return c => BottomBar(c).Descendant().Text(LabelExplore); } }

        public override Func<AppQuery, AppQuery> MyContentMenuItem { get { return c => BottomBar(c).Descendant().Text(LabelMyContent); } }

        public override Func<AppQuery, AppQuery> LibraryMenuItem { get { return c => BottomBar(c).Descendant().Text(LabelLibrary); } }

        public override Func<AppQuery, AppQuery> ProfileMenuItem { get { return c => BottomBar(c).Descendant().Text(LabelProfile); } }

        public override Func<AppQuery, AppQuery> GermanProfileMenuItem { get { return c => BottomBar(c).Descendant().Text("Profil"); } }
    }
}