using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Contributors;
using MvvmCross;

namespace BMM.Core.ViewModels
{
    public class ExploreContributorsViewModel : LoadMoreDocumentsViewModel
    {
        // Kåre is supposed to always be the first entry of the list
        private readonly List<int> _orderedListStart = new List<int>
        {
            36491, //Kåre J. Smith
            49489, //Elias Aslaksen
        };

        private readonly IEnumerable<int> _alphabetical = new List<int>()
        {
            66376, //Aksel J. Smith
            92568, //Alise Johnsen
            36514, //Arild Tombre
            59268, //Astrid Reinhardt
            60455, //Atle Johnsen
            36501, //Bernt Stadven
            36503, //Bjørn Nilsen
            41600, //Dag Helge Bernhardsen
            75152, //Elisa Frivold
            36562, //Gershon Twilley
            45275, //Gjermund Frivold
            36515, //Gunnar Gangsø
            51294, //Hanne Trinkle
            36522, //Harald Kronstad
            41642, //Hilde Smith Stadven
            65224, //Jermund Pedersen
            41598, //Jostein Ostmoen
            49935, //Karethe Opitz
            49933, //Kristiane Opitz
            45272, //Linn Helgheim
            41622, //Liv Ragnhild Fotland
            60845, //Marte Hannson
            59596, //Oliver Tangen
            80142, //Pia Gjosund
            64808, //Pia Veronica Jacobsen
            81631, //Rebekka Frivold
            49809, //Sigurd Bratlie
            36517, //Sverre Riksfjord
            36519, //Trond Eriksen
            90712, //Trygve Sandvik
            60844  //Vegar Sandberg
        };

        private readonly List<int> _randomContributors = new List<int>();

        private readonly IAnalytics _analytics;

        public ExploreContributorsViewModel(IAnalytics analytics, ITrackPOFactory trackPOFactory) : base(trackPOFactory)
        {
            _analytics = analytics;
        }

        protected override Task Initialization()
        {
            //IEnumerable<int> a = _singers;
            //IEnumerable<int> b = _speekers;
            //IEnumerable<int> c = a.Union(b);
            //_randomContributors.AddRange(c);
            //int total = _randomContributors.Count;

            //Random rng = new Random();

            //while (total > 1)
            //{
            //    total--;
            //    int k = rng.Next(total + 1);
            //    int value = _randomContributors[k];
            //    _randomContributors[k] = _randomContributors[total];
            //    _randomContributors[total] = value;
            //}

            _randomContributors.InsertRange(0, _orderedListStart);
            _randomContributors.AddRange(_alphabetical);

            return base.Initialization();
        }

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(int startIndex, int size, CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var contributors = new List<Contributor>();
            if (startIndex < _randomContributors.Count)
            {
                int newSize = startIndex + size < _randomContributors.Count ? size : _randomContributors.Count - size;
                contributors.AddRange(await LoadContributors(_randomContributors.GetRange(startIndex, newSize)));
            }

            /*
             * Detect situation that all contributors are loaded in optimistic scenario and try to load missing items if they exist.
             * Problem with missing items occurs when user have problem with network connection and tasks was canceled.
             */
            if (contributors.Count == 0)
            {
                var missingIds = _randomContributors.Where(r => Documents.All(d => d.Id != r)).ToList();
                if (missingIds.Any())
                {
                    contributors.AddRange(await LoadContributors(missingIds));
                    _analytics.LogEvent("4932 Add missing contributors");
                }
            }

            contributors.RemoveAll(c => c == null);
            return contributors.Select(c => new ContributorPO(OptionCommand, c));
        }

        private async Task<List<Contributor>> LoadContributors(IEnumerable<int> ids)
        {
            var contributorTasks = new List<Task<Contributor>>();
            foreach (var id in ids)
            {
                try
                {
                    contributorTasks.Add(Client.Contributors.GetById(id));
                }
                catch (Exception ex)
                {
                    ExceptionHandler.HandleException(ex);
                }
            }

            return (await Task.WhenAll(contributorTasks)).ToList();
        }

        protected override async Task DocumentAction(IDocumentPO item, IList<Track> list)
        {
            if (item.DocumentType == DocumentType.Contributor)
            {
                Mvx.IoCProvider.Resolve<IAnalytics>().LogEvent("User interacts with Single Contributor view");
            }

            await base.DocumentAction(item, list);
        }
    }
}