// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/ShipControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @ShipControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @ShipControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""ShipControls"",
    ""maps"": [
        {
            ""name"": ""Move"",
            ""id"": ""1f66100b-30a0-4b74-9d19-3c7ffcd7ce10"",
            ""actions"": [
                {
                    ""name"": ""WASD"",
                    ""type"": ""Value"",
                    ""id"": ""ad5638c0-c265-491f-b24b-9a8c0ed0e030"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Quit"",
                    ""type"": ""Value"",
                    ""id"": ""ded34506-53e2-4bc0-ac73-e19bf03a8044"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""16c8e566-828e-46cf-8221-00083eaae323"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1c89a909-6e64-4425-ab40-7ca92132d650"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""9bf30ba6-92ea-4873-89f9-98953bad8437"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""5ebe305b-c8b0-4b20-82d8-0103e510e528"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""94e2f77e-6d48-45dc-9976-9132cc5bc689"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""94e99033-af1c-43f9-ae74-41830c00684f"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Mouse"",
            ""id"": ""ed54cfee-0bbd-4c97-a10a-a4e4882eba09"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""06e28c59-a415-44ec-9cfd-d6ac5d51333f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""193dd798-24f5-4392-be49-b82b7f5ca359"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Move
        m_Move = asset.FindActionMap("Move", throwIfNotFound: true);
        m_Move_WASD = m_Move.FindAction("WASD", throwIfNotFound: true);
        m_Move_Quit = m_Move.FindAction("Quit", throwIfNotFound: true);
        // Mouse
        m_Mouse = asset.FindActionMap("Mouse", throwIfNotFound: true);
        m_Mouse_Newaction = m_Mouse.FindAction("New action", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Move
    private readonly InputActionMap m_Move;
    private IMoveActions m_MoveActionsCallbackInterface;
    private readonly InputAction m_Move_WASD;
    private readonly InputAction m_Move_Quit;
    public struct MoveActions
    {
        private @ShipControls m_Wrapper;
        public MoveActions(@ShipControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @WASD => m_Wrapper.m_Move_WASD;
        public InputAction @Quit => m_Wrapper.m_Move_Quit;
        public InputActionMap Get() { return m_Wrapper.m_Move; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MoveActions set) { return set.Get(); }
        public void SetCallbacks(IMoveActions instance)
        {
            if (m_Wrapper.m_MoveActionsCallbackInterface != null)
            {
                @WASD.started -= m_Wrapper.m_MoveActionsCallbackInterface.OnWASD;
                @WASD.performed -= m_Wrapper.m_MoveActionsCallbackInterface.OnWASD;
                @WASD.canceled -= m_Wrapper.m_MoveActionsCallbackInterface.OnWASD;
                @Quit.started -= m_Wrapper.m_MoveActionsCallbackInterface.OnQuit;
                @Quit.performed -= m_Wrapper.m_MoveActionsCallbackInterface.OnQuit;
                @Quit.canceled -= m_Wrapper.m_MoveActionsCallbackInterface.OnQuit;
            }
            m_Wrapper.m_MoveActionsCallbackInterface = instance;
            if (instance != null)
            {
                @WASD.started += instance.OnWASD;
                @WASD.performed += instance.OnWASD;
                @WASD.canceled += instance.OnWASD;
                @Quit.started += instance.OnQuit;
                @Quit.performed += instance.OnQuit;
                @Quit.canceled += instance.OnQuit;
            }
        }
    }
    public MoveActions @Move => new MoveActions(this);

    // Mouse
    private readonly InputActionMap m_Mouse;
    private IMouseActions m_MouseActionsCallbackInterface;
    private readonly InputAction m_Mouse_Newaction;
    public struct MouseActions
    {
        private @ShipControls m_Wrapper;
        public MouseActions(@ShipControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_Mouse_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_Mouse; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MouseActions set) { return set.Get(); }
        public void SetCallbacks(IMouseActions instance)
        {
            if (m_Wrapper.m_MouseActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnNewaction;
            }
            m_Wrapper.m_MouseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
            }
        }
    }
    public MouseActions @Mouse => new MouseActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface IMoveActions
    {
        void OnWASD(InputAction.CallbackContext context);
        void OnQuit(InputAction.CallbackContext context);
    }
    public interface IMouseActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}
