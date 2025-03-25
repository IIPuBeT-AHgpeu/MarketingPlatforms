using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarketingPlatforms.Infrastructure.Tests
{
    [TestClass()]
    public class ConfigurationRepositoryMethodsTests
    {
        private ConfigurationRepository _repForTestGetMethod;
        public ConfigurationRepositoryMethodsTests() 
        {
            string[] testData =
                [
                    "Яндекс.Директ:/ru",
                    "Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik",
                    "Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl",
                    "Крутая реклама:/ru/svrd",
                    "Реклама Волгограда:/ru/vlg,/ru/vlzh",
                    "Вестник:/ru",
                    "Волжский рекламщик:/ru/vlzh"
                ];
            _repForTestGetMethod = new ConfigurationRepository();
            var isSuccessfully = _repForTestGetMethod.TrySetValue(testData);
        }

        [TestMethod()]
        public void TrySetValueTest_LikeExample()
        {
            var rep = new ConfigurationRepository();

            string[] testData =
                [
                    "Яндекс.Директ:/ru",
                    "Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik",
                    "Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl",
                    "Крутая реклама:/ru/svrd"
                ];

            var isSuccessfully = rep.TrySetValue(testData);
            var expected = new Dictionary<string, List<string>>();

            expected.Add("/ru", new List<string>() { "Яндекс.Директ" });
            expected.Add("/ru/svrd/revda", new List<string>() { "Ревдинский рабочий" });
            expected.Add("/ru/svrd/pervik", new List<string>() { "Ревдинский рабочий" });
            expected.Add("/ru/msk", new List<string>() { "Газета уральских москвичей" });
            expected.Add("/ru/permobl", new List<string>() { "Газета уральских москвичей" });
            expected.Add("/ru/chelobl", new List<string>() { "Газета уральских москвичей" });
            expected.Add("/ru/svrd", new List<string>() { "Крутая реклама" });

            Assert.IsNotNull(rep.Config);
            Assert.IsTrue(DictionaryComparer(expected, rep.Config));
        }

        [TestMethod()]
        public void TrySetValueTest_WithDuplicates()
        {
            var rep = new ConfigurationRepository();

            string[] testData =
                [
                    "Яндекс.Директ:/ru",
                    "Яндекс.Директ:/ru",
                    "Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik",
                    "Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl",
                    "Крутая реклама:/ru/svrd",
                    "Яндекс.Директ:/ru"
                ];

            var isSuccessfully = rep.TrySetValue(testData);
            var expected = new Dictionary<string, List<string>>();

            expected.Add("/ru", new List<string>() { "Яндекс.Директ" });
            expected.Add("/ru/svrd/revda", new List<string>() { "Ревдинский рабочий" });
            expected.Add("/ru/svrd/pervik", new List<string>() { "Ревдинский рабочий" });
            expected.Add("/ru/msk", new List<string>() { "Газета уральских москвичей" });
            expected.Add("/ru/permobl", new List<string>() { "Газета уральских москвичей" });
            expected.Add("/ru/chelobl", new List<string>() { "Газета уральских москвичей" });
            expected.Add("/ru/svrd", new List<string>() { "Крутая реклама" });

            Assert.IsNotNull(rep.Config);
            Assert.IsTrue(DictionaryComparer(expected, rep.Config));
        }

        [TestMethod()]
        public void TrySetValueTest_WithManyPlatformsOnOneLocation()
        {
            var rep = new ConfigurationRepository();

            string[] testData =
                [
                    "Яндекс.Директ:/ru",
                    "Ваша реклама:/ru",
                    "Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik",
                    "Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl",
                    "Крутая реклама:/ru/svrd",
                    "Всероссийский рекламодатель:/ru"
                ];

            var isSuccessfully = rep.TrySetValue(testData);
            var expected = new Dictionary<string, List<string>>();

            expected.Add("/ru/svrd", new List<string>() { "Крутая реклама" });
            expected.Add("/ru", new List<string>() { "Яндекс.Директ", "Ваша реклама", "Всероссийский рекламодатель" });
            expected.Add("/ru/svrd/revda", new List<string>() { "Ревдинский рабочий" });
            expected.Add("/ru/svrd/pervik", new List<string>() { "Ревдинский рабочий" });
            expected.Add("/ru/msk", new List<string>() { "Газета уральских москвичей" });
            expected.Add("/ru/permobl", new List<string>() { "Газета уральских москвичей" });
            expected.Add("/ru/chelobl", new List<string>() { "Газета уральских москвичей" });           

            Assert.IsNotNull(rep.Config);
            Assert.IsTrue(DictionaryComparer(expected, rep.Config));
        }

        [TestMethod()]
        public void TrySetValueTest_EmptyInput()
        {
            var rep = new ConfigurationRepository();

            string[] testData = [];

            var isSuccessfully = rep.TrySetValue(testData);

            Assert.IsNotNull(rep.Config);
            Assert.IsTrue(rep.Config.Count == 0);
        }

        [TestMethod()]
        public void GetDataByKeyTest_ItHasNoPlatforms()
        {
            var actual = _repForTestGetMethod.GetDataByKey("/kz/ast");

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count() == 0);
        }

        [TestMethod()]
        public void GetDataByKeyTest_GetFourPlatforms()
        {
            var actual = _repForTestGetMethod.GetDataByKey("/ru/svrd/revda");
            var expected = new List<string>() { "Яндекс.Директ", "Ревдинский рабочий", "Крутая реклама", "Вестник" };

            Assert.IsNotNull(actual);
            Assert.IsTrue(ListComparer(actual.ToList(), expected));
        }

        private bool DictionaryComparer(Dictionary<string, List<string>> first, Dictionary<string, List<string>> second)
        {
            if (first == null || second == null) return false;
            if (first.Count != second.Count) return false;

            foreach (var item in first)
            {
                if (second.ContainsKey(item.Key))
                {
                    if (!ListComparer(item.Value, second[item.Key])) return false;
                }
                else return false;
            }

            return true;
        }

        private bool ListComparer(List<string> first, List<string> second)
        {
            if (first == null || second == null) return false;
            if (first.Count != second.Count) return false;

            foreach (var item in first) if (!second.Contains(item)) return false;

            return true;
        }
    }
}