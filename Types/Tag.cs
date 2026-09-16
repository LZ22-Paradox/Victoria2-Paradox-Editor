namespace Paradox_Editor.Types;

public readonly record struct Tag
{
    public Tag(string value) => Value = value;

    public string Value { get; init; }
    public static implicit operator string(Tag tag) => tag.Value;
    public static implicit operator Tag(string tag) => new(tag);
}