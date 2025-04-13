using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TicketAppDesktop.DataLayer;
using TicketAppDesktop.Models;

namespace TicketAppDesktop.ViewModels
{
    public class GroupViewModel : INotifyPropertyChanged
    {
        private string _groupName = string.Empty;
        private string _description = string.Empty;
        private string _managerId = string.Empty;
        private ObservableCollection<Group> _groups = new ObservableCollection<Group>();

        // Properties bound to the "add group" section of your UI.
        public string GroupName
        {
            get => _groupName;
            set { _groupName = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public string ManagerId
        {
            get => _managerId;
            set { _managerId = value; OnPropertyChanged(); }
        }

        // Collection bound to a DataGrid (or similar control) to display all groups.
        public ObservableCollection<Group> Groups
        {
            get => _groups;
            private set { _groups = value; OnPropertyChanged(); }
        }

        // Constructor loads any existing groups.
        public GroupViewModel()
        {
            // Retrieve existing groups from the DAL. 
            Groups = new ObservableCollection<Group>(GroupsDAL.RetrieveAllGroups()!);
        }

        // Adds a new group and updates the UI.
        public void AddGroup()
        {

            // Create a new Group model (assumes a constructor like the one shown).
            var newGroup = new Group(

                id: Guid.NewGuid().ToString(),
                groupName: GroupName,
                description: Description,
                managerId: ManagerId,
                createdAt: DateTime.UtcNow);

            // Send the new group to the data layer.
            GroupsDAL.SaveGroup(newGroup);

            // Add the new group to the observable collection.
            Groups.Add(newGroup);
        }

        // Optionally, refresh the entire group list from the data layer.
        public void RefreshGroups()
        {
            Groups = new ObservableCollection<Group>(GroupsDAL.RetrieveAllGroups());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)

            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}