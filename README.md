# CustomRichTextBox

A Windows Forms `RichTextBox` control extended with **print support** and **drag-and-drop text**, available in both VB.NET (.NET 4.8) and C# (.NET 5) editions.

**Source last updated:** 2021-09-12
**Initiated:** 2015-03-02 · **Solution:** `VaderConsulting.CustomRichTextBox.sln`

---

## Overview

The standard `RichTextBox` provides no built-in printing. This library adds printing via the Win32 `EM_FORMATRANGE` message, enabling proper multi-page rendering to a printer device context. Originally based on Microsoft KB article 811401.

---

## Projects

| Project | Language | Target |
|---------|----------|--------|
| `VaderConsulting.CustomRichTextBox_vb` | VB.NET | .NET Framework 4.8 |
| `CustomRichTextbox_5` | C# | .NET 5 |

---

## Features

- **Printing** - `Print(int charFrom, int charTo, PrintPageEventArgs e)` renders a range of characters to a printer DC via `EM_FORMATRANGE`. Returns the last character printed + 1 for multi-page looping.
- **Drag-and-drop** - Handles `DragEnter` (accepts `DataFormats.Text`) and `DragDrop` (inserts text at the caret).

---

## Usage

```csharp
int charFrom = 0;
PrintDocument doc = new PrintDocument();
doc.PrintPage += (s, e) =>
{
    charFrom = richTextBox1.Print(charFrom, richTextBox1.TextLength, e);
    e.HasMorePages = (charFrom < richTextBox1.TextLength);
};
doc.Print();
```

## Requirements

- Visual Studio 2019, .NET 5.0, .NET Framework 3.5

