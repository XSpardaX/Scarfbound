BUTTON_GUID = "67ca024dc25229644b2a54f9725d46a7"
FONT_GUID = "e48d81020762c7946ac70f95631c3c78"
MENU_GUID = "daf512eac987d064b8c00f6a6a532e86"


def button_block(inst_id, rect_id, parent_id, name, label, y, width=300):
    return f"""--- !u!1001 &{inst_id}
PrefabInstance:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_Modification:
    serializedVersion: 3
    m_TransformParent: {{fileID: {parent_id}}}
    m_Modifications:
    - target: {{fileID: 3600489594026726489, guid: {BUTTON_GUID}, type: 3}}
      propertyPath: m_Name
      value: {name}
      objectReference: {{fileID: 0}}
    - target: {{fileID: 4522747612506524011, guid: {BUTTON_GUID}, type: 3}}
      propertyPath: m_text
      value: {label}
      objectReference: {{fileID: 0}}
    - target: {{fileID: 6448993961539896978, guid: {BUTTON_GUID}, type: 3}}
      propertyPath: m_Pivot.x
      value: 0.5
      objectReference: {{fileID: 0}}
    - target: {{fileID: 6448993961539896978, guid: {BUTTON_GUID}, type: 3}}
      propertyPath: m_Pivot.y
      value: 0.5
      objectReference: {{fileID: 0}}
    - target: {{fileID: 6448993961539896978, guid: {BUTTON_GUID}, type: 3}}
      propertyPath: m_AnchorMax.x
      value: 0.5
      objectReference: {{fileID: 0}}
    - target: {{fileID: 6448993961539896978, guid: {BUTTON_GUID}, type: 3}}
      propertyPath: m_AnchorMax.y
      value: 0.5
      objectReference: {{fileID: 0}}
    - target: {{fileID: 6448993961539896978, guid: {BUTTON_GUID}, type: 3}}
      propertyPath: m_AnchorMin.x
      value: 0.5
      objectReference: {{fileID: 0}}
    - target: {{fileID: 6448993961539896978, guid: {BUTTON_GUID}, type: 3}}
      propertyPath: m_AnchorMin.y
      value: 0.5
      objectReference: {{fileID: 0}}
    - target: {{fileID: 6448993961539896978, guid: {BUTTON_GUID}, type: 3}}
      propertyPath: m_SizeDelta.x
      value: {width}
      objectReference: {{fileID: 0}}
    - target: {{fileID: 6448993961539896978, guid: {BUTTON_GUID}, type: 3}}
      propertyPath: m_SizeDelta.y
      value: 59.333332
      objectReference: {{fileID: 0}}
    - target: {{fileID: 6448993961539896978, guid: {BUTTON_GUID}, type: 3}}
      propertyPath: m_AnchoredPosition.x
      value: 0
      objectReference: {{fileID: 0}}
    - target: {{fileID: 6448993961539896978, guid: {BUTTON_GUID}, type: 3}}
      propertyPath: m_AnchoredPosition.y
      value: {y}
      objectReference: {{fileID: 0}}
    m_RemovedComponents: []
    m_RemovedGameObjects: []
    m_AddedGameObjects: []
    m_AddedComponents: []
  m_SourcePrefab: {{fileID: 100100000, guid: {BUTTON_GUID}, type: 3}}
--- !u!224 &{rect_id} stripped
RectTransform:
  m_CorrespondingSourceObject: {{fileID: 6448993961539896978, guid: {BUTTON_GUID}, type: 3}}
  m_PrefabInstance: {{fileID: {inst_id}}}
  m_PrefabAsset: {{fileID: 0}}
"""


def title_block(go_id, rect_id, cr_id, tmp_id, parent_id, text, y, size=52):
    return f"""--- !u!1 &{go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {rect_id}}}
  - component: {{fileID: {cr_id}}}
  - component: {{fileID: {tmp_id}}}
  m_Layer: 5
  m_Name: Title
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &{rect_id}
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: {parent_id}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0.5, y: 0.5}}
  m_AnchorMax: {{x: 0.5, y: 0.5}}
  m_AnchoredPosition: {{x: 0, y: {y}}}
  m_SizeDelta: {{x: 800, y: 100}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!222 &{cr_id}
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_CullTransparentMesh: 1
--- !u!114 &{tmp_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: f4688fdb7df04437aeb418b961361dc5, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_text: {text}
  m_fontAsset: {{fileID: 11400000, guid: {FONT_GUID}, type: 2}}
  m_sharedMaterial: {{fileID: 3403671416660078228, guid: {FONT_GUID}, type: 2}}
  m_fontSize: {size}
  m_fontSizeBase: {size}
  m_fontColor: {{r: 1, g: 1, b: 1, a: 1}}
  m_HorizontalAlignment: 2
  m_VerticalAlignment: 512
  m_isOrthographic: 1
"""


def overlay_panel(go_id, rect_id, cr_id, img_id, canvas_id, ray_id, parent_id, children, sort=100):
    child_lines = "\n".join(f"  - {{fileID: {child}}}" for child in children)
    return f"""--- !u!1 &{go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {rect_id}}}
  - component: {{fileID: {cr_id}}}
  - component: {{fileID: {img_id}}}
  - component: {{fileID: {canvas_id}}}
  - component: {{fileID: {ray_id}}}
  m_Layer: 5
  m_Name: Overlay
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &{rect_id}
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
{child_lines}
  m_Father: {{fileID: {parent_id}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0, y: 0}}
  m_AnchorMax: {{x: 1, y: 1}}
  m_AnchoredPosition: {{x: 0, y: 0}}
  m_SizeDelta: {{x: 0, y: 0}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!222 &{cr_id}
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_CullTransparentMesh: 1
--- !u!114 &{img_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: fe87c0e1cc204ed48ad3b37840f39efc, type: 3}}
  m_Color: {{r: 0, g: 0, b: 0, a: 0.78}}
  m_Sprite: {{fileID: 10907, guid: 0000000000000000f000000000000000, type: 0}}
  m_Type: 1
--- !u!223 &{canvas_id}
Canvas:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: 1
  serializedVersion: 3
  m_RenderMode: 0
  m_OverrideSorting: 1
  m_SortingOrder: {sort}
--- !u!114 &{ray_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: 1
  m_Script: {{fileID: 11500000, guid: dc42784cf147c0c48a680349fa168899, type: 3}}
"""


def main():
    parts = [
        "%YAML 1.1",
        "%TAG !u! tag:unity3d.com,2011:",
        """--- !u!1 &800000001
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 800000002}
  - component: {fileID: 800000003}
  m_Layer: 5
  m_Name: MainMenuUI
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &800000002
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 800000001}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {fileID: 800000102}
  - {fileID: 800000202}
  - {fileID: 800000302}
  m_Father: {fileID: 0}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
  m_AnchorMin: {x: 0, y: 0}
  m_AnchorMax: {x: 1, y: 1}
  m_AnchoredPosition: {x: 0, y: 0}
  m_SizeDelta: {x: 0, y: 0}
  m_Pivot: {x: 0.5, y: 0.5}
--- !u!114 &800000003
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 800000001}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: """
        + MENU_GUID
        + """, type: 3}
  m_Name: 
  m_EditorClassIdentifier: 
  controlsScreen: {fileID: 800000301}
  mainPanel: {fileID: 800000101}
  levelSelect: {fileID: 800000201}
""",
        """--- !u!1 &800000101
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 800000102}
  m_Layer: 5
  m_Name: MainPanel
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &800000102
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 800000101}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {fileID: 800000111}
  - {fileID: 801001002}
  - {fileID: 801002002}
  - {fileID: 801003002}
  m_Father: {fileID: 800000002}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
  m_AnchorMin: {x: 0, y: 0}
  m_AnchorMax: {x: 1, y: 1}
  m_AnchoredPosition: {x: 0, y: 0}
  m_SizeDelta: {x: 0, y: 0}
  m_Pivot: {x: 0.5, y: 0.5}
""",
        title_block(800000110, 800000111, 800000112, 800000113, 800000102, "SCARFBOUND", 180, 64),
        button_block(801001001, 801001002, 800000102, "PlayButton", "Play", 60),
        button_block(801002001, 801002002, 800000102, "InstructionsButton", "How To Play", -20),
        button_block(801003001, 801003002, 800000102, "QuitButton", "Quit Game", -100),
        """--- !u!1 &800000201
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 800000202}
  m_Layer: 5
  m_Name: LevelSelectPanel
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 0
--- !u!224 &800000202
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 800000201}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {fileID: 800000212}
  m_Father: {fileID: 800000002}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
  m_AnchorMin: {x: 0, y: 0}
  m_AnchorMax: {x: 1, y: 1}
  m_AnchoredPosition: {x: 0, y: 0}
  m_SizeDelta: {x: 0, y: 0}
  m_Pivot: {x: 0.5, y: 0.5}
""",
        overlay_panel(
            800000211,
            800000212,
            800000213,
            800000214,
            800000215,
            800000216,
            800000202,
            [800000221, 802001002, 802002002, 802003002, 802004002],
        ),
        title_block(800000220, 800000221, 800000222, 800000223, 800000212, "SELECT LEVEL", 140, 48),
        button_block(802001001, 802001002, 800000212, "Level1Button", "Level 1", 40, 260),
        button_block(802002001, 802002002, 800000212, "Level2Button", "Level 2", -40, 260),
        button_block(802003001, 802003002, 800000212, "Level3Button", "Level 3", -120, 260),
        button_block(802004001, 802004002, 800000212, "LevelSelectBackButton", "Back", -220, 220),
        """--- !u!1 &800000301
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 800000302}
  m_Layer: 5
  m_Name: ControlsPanel
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 0
--- !u!224 &800000302
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 800000301}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {fileID: 800000312}
  m_Father: {fileID: 800000002}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
  m_AnchorMin: {x: 0, y: 0}
  m_AnchorMax: {x: 1, y: 1}
  m_AnchoredPosition: {x: 0, y: 0}
  m_SizeDelta: {x: 0, y: 0}
  m_Pivot: {x: 0.5, y: 0.5}
""",
        overlay_panel(
            800000311,
            800000312,
            800000313,
            800000314,
            800000315,
            800000316,
            800000302,
            [800000321, 800000331, 803001002],
            101,
        ),
        title_block(800000320, 800000321, 800000322, 800000323, 800000312, "HOW TO PLAY", 220, 44),
    ]

    body_text = (
        "How to Play\\n\\nWASD - movement\\nSPACE - jump\\nMOUSE - look around\\n\\n"
        "Story\\n\\nIn the depths of the Underworld, faded spirits called umbrids wander "
        "without identity. Over time they develop a bright red scarf granting them a single "
        "simple instinct: climb upward and escape."
    )
    parts.append(
        f"""--- !u!1 &800000330
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 800000331}}
  - component: {{fileID: 800000332}}
  - component: {{fileID: 800000333}}
  m_Layer: 5
  m_Name: BodyText
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &800000331
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 800000330}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 800000312}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0.5, y: 0.5}}
  m_AnchorMax: {{x: 0.5, y: 0.5}}
  m_AnchoredPosition: {{x: 0, y: 20}}
  m_SizeDelta: {{x: 900, y: 420}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!222 &800000332
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 800000330}}
  m_CullTransparentMesh: 1
--- !u!114 &800000333
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 800000330}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: f4688fdb7df04437aeb418b961361dc5, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_text: {body_text}
  m_fontAsset: {{fileID: 11400000, guid: {FONT_GUID}, type: 2}}
  m_sharedMaterial: {{fileID: 3403671416660078228, guid: {FONT_GUID}, type: 2}}
  m_fontSize: 22
  m_fontSizeBase: 22
  m_fontColor: {{r: 1, g: 1, b: 1, a: 1}}
  m_HorizontalAlignment: 2
  m_VerticalAlignment: 256
  m_isOrthographic: 1
"""
    )
    parts.append(button_block(803001001, 803001002, 800000312, "ControlsBackButton", "Back", -260, 220))

    output = r"D:\Unity projects\Scarfbound\Assets\Prefab\MainMenuUI.prefab"
    with open(output, "w", encoding="utf-8") as handle:
        handle.write("\n".join(parts))
    print(f"Wrote {output}")


if __name__ == "__main__":
    main()
