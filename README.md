# inz-unity-ar

This repository contains scripts used in the Unity AR project. The `MessageLog` component
provides a scrollable UI log useful for debug output or chat style messages.

## Setting up the Scrollable Log

1. **Create the UI hierarchy**
   - Add a **Scroll View** (``ScrollRect``) to your Canvas.
   - Remove the default ``Scrollbar`` objects if you don't need them.
   - Inside the Scroll View's **Viewport**, add a ``Text`` or ``TMP_Text`` component
     as the only child. Resize it so it expands vertically.

2. **Add the `MessageLog` component**
   - Create an empty GameObject or use the Scroll View itself.
   - Attach the ``MessageLog`` script.
   - Drag the Scroll Rect and Text/TMP Text references into the corresponding
     fields in the inspector.

3. **Logging messages**
   - Call ``AddMessage(string)`` on the ``MessageLog`` component to append new
     lines to the log. The log automatically keeps the most recent messages
     visible.
   - You can adjust the ``Max Messages`` field in the inspector to limit how
     many messages are stored. Older entries are discarded once the limit is
     reached.

This component stores the messages internally and concatenates them with newline
separators before updating the UI.
