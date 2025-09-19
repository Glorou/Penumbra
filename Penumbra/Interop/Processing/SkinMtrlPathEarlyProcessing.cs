using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Client.System.Resource.Handle;

namespace Penumbra.Interop.Processing;

public static unsafe class SkinMtrlPathEarlyProcessing
{
    public static void Process(Span<byte> path, CharacterBase* character, uint slotIndex)
    {
        var end = path.IndexOf(MaterialExtension());
        if (end < 0)
            return;

        var suffixPos = path[..end].LastIndexOf((byte)'_');
        if (suffixPos < 0)
            return;

        var handle = GetModelResourceHandle(character, slotIndex);
        if (handle == null)
            return;

        var skinSuffix = GetSkinSuffix(handle);
        if (skinSuffix.IsEmpty || skinSuffix.Length > path.Length - suffixPos - 7)
            return;

        ++suffixPos;
        skinSuffix.CopyTo(path[suffixPos..]);
        suffixPos += skinSuffix.Length;
        MaterialExtension().CopyTo(path[suffixPos..]);
        return;

        static ReadOnlySpan<byte> MaterialExtension()
            => ".mtrl\0"u8;
    }

    private static ModelResourceHandle* GetModelResourceHandle(CharacterBase* character, uint slotIndex)
    {
        if (character is null)
            return null;

        if (character->PerSlotStagingArea is not null)
        {
            var handle = character->PerSlotStagingArea[slotIndex].ModelResourceHandle;
            if (handle != null)
                return handle;
        }

        var model = character->Models[slotIndex];
        return model is null ? null : model->ModelResourceHandle;
    }

    private static ReadOnlySpan<byte> GetSkinSuffix(ModelResourceHandle* handle)
    {
        foreach (var (attribute, _) in handle->Attributes)
        {
            var attributeSpan = attribute.AsSpan();
            if (attributeSpan.Length > 12 && attributeSpan[..11].SequenceEqual("skin_suffix"u8) && attributeSpan[11] is (byte)'=' or (byte)'_')
                return attributeSpan[12..];
        }

        return [];
    }
}
