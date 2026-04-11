using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace DonutMessager.Helpers
{
    public static class LocalUsers
    {
        public static List<int> Get()
        {
            var raw = Properties.Settings.Default.LoggedUsers;
            if (string.IsNullOrWhiteSpace(raw))
                return new List<int>();

            return raw.Split(',').Select(int.Parse).ToList();
        }

        public static void Add(int id)
        {
            var list = Get();
            if (!list.Contains(id))
                list.Add(id);

            Properties.Settings.Default.LoggedUsers = string.Join(",", list);
            Properties.Settings.Default.Save();
        }

        public static void Remove(int id)
        {
            var list = Get();
            if (list.Contains(id))
                list.Remove(id);

            Properties.Settings.Default.LoggedUsers = string.Join(",", list);
            Properties.Settings.Default.Save();
        }
    }
}
