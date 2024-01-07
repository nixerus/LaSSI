using Eto.Drawing;
using Eto.Forms;
using Microsoft.VisualBasic;

namespace LaSSI.Schema;

public class SchemaSelectWindow : Dialog<string>
{
    public SchemaSelectWindow(Oncler oncler, string[] options)
    {
        Closeable = false;
        Padding = new Padding(25, 25);
        Title = oncler.Key;
        
        var layout = new DynamicLayout();
        layout.DefaultSpacing = new Size(5, 5);
        var dropdown = new DropDown
        {
            ID = "selection"
        };

        var matchingFound = false;
        for (int i = 0; i < options.Length; i++)
        {
            var option = options[i];
            var item = new ListItem
            {
                Key = i.ToString(),
                Text = option
            };
            dropdown.Items.Add(item);
            
            if (oncler.Value != null && oncler.Value.Equals(option))
            {
                dropdown.SelectedIndex = i;
                matchingFound = true;
            }
        }
        
        dropdown.Items.Add(new ListItem
        {
            Key = options.Length.ToString(),
            Text = "Custom"
        });

        if (!matchingFound)
            dropdown.SelectedIndex = dropdown.Items.Count - 1;

        var text = new TextBox
        {
            ID = "custom-text",
            Enabled = !matchingFound
        };

        dropdown.SelectedIndexChanged += ((sender, args) =>
        {
            text.Enabled = dropdown.SelectedIndex == dropdown.Items.Count - 1;
            if (!text.Enabled)
                text.Text = string.Empty;
        });

        var button = new Button
        {
            Text = "Save"
        };

        button.Click += (sender, args) =>
        {
            Close(text.Enabled ? text.Text : dropdown.Items[dropdown.SelectedIndex].Text);
        };
        
        layout.AddCentered(dropdown);
        layout.AddCentered(text);
        layout.AddSpace();
        layout.AddCentered(button);
        Content = layout;
    }
}