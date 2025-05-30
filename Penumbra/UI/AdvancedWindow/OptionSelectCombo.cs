using ImGuiNET;
using OtterGui.Raii;
using OtterGui.Text;
using OtterGui.Widgets;
using Penumbra.Mods.Editor;
using Penumbra.UI.Classes;

namespace Penumbra.UI.AdvancedWindow;

public sealed class OptionSelectCombo(ModEditor editor, ModEditWindow window)
    : FilterComboCache<(string FullName, (int Group, int Data) Index)>(
        () => window.Mod!.AllDataContainers.Select(c => (c.GetFullName(), c.GetDataIndices())).ToList(), MouseWheelType.Control, Penumbra.Log)
{
    private ImRaii.ColorStyle _border;

    protected override void DrawCombo(string label, string preview, string tooltip, int currentSelected, float previewWidth, float itemHeight,
        ImGuiComboFlags flags)
    {
        _border = ImRaii.PushFrameBorder(ImUtf8.GlobalScale, ColorId.FolderLine.Value());
        base.DrawCombo(label, preview, tooltip, currentSelected, previewWidth, itemHeight, flags);
        _border.Dispose();
    }

    protected override void DrawFilter(int currentSelected, float width)
    {
        _border.Dispose();
        base.DrawFilter(currentSelected, width);
    }

    public bool Draw(float width)
    {
        var flags = window.Mod!.AllDataContainers.Count() switch
        {
            0   => ImGuiComboFlags.NoArrowButton,
            > 8 => ImGuiComboFlags.HeightLargest,
            _   => ImGuiComboFlags.None,
        };
        return Draw("##optionSelector", editor.Option!.GetFullName(), string.Empty, width, ImGui.GetTextLineHeight(), flags);
    }

    protected override bool DrawSelectable(int globalIdx, bool selected)
        => ImUtf8.Selectable(Items[globalIdx].FullName, selected);
}
