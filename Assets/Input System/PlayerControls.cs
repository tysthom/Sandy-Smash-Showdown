//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Input System/PlayerControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""4915dddb-9d0b-4a4a-b5dd-8260532d3aad"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""20cc03fa-b8fe-41a1-81a4-8ce4523b568f"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Serve"",
                    ""type"": ""Button"",
                    ""id"": ""73e89a97-5325-4dfc-9a6a-49c4a69d8cef"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""436b875c-a23a-4956-bf85-4df003ff435b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Spike"",
                    ""type"": ""Button"",
                    ""id"": ""114ef9d2-f8b3-47f9-b841-006c9b61b714"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Dig"",
                    ""type"": ""Button"",
                    ""id"": ""29af3918-ddd7-4cfe-9d92-d407735ab7cb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Bump"",
                    ""type"": ""Button"",
                    ""id"": ""87b4f3c0-bcca-4f7b-ba22-47b0cb5b19ee"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Set"",
                    ""type"": ""Button"",
                    ""id"": ""db916ba9-aaf7-43ef-8dd3-f71cc20e618b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b935223f-1895-4876-9ee4-c250f9dea3e2"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""69f65356-65ab-4ac0-8ec9-8390dd12000f"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Serve"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""09e90c41-4944-415f-992b-7e9af6633e3f"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dig"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c11e1d46-928f-46eb-8436-7b46b84d713f"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2e1f1ab5-92d7-4ff2-9eef-b935ed9bb9a9"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Spike"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a5984b63-d8ca-436b-8355-407d14da0d8b"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Bump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f3443cc2-28b5-4cba-a8a2-8d2064959535"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Set"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Serve = m_Player.FindAction("Serve", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_Spike = m_Player.FindAction("Spike", throwIfNotFound: true);
        m_Player_Dig = m_Player.FindAction("Dig", throwIfNotFound: true);
        m_Player_Bump = m_Player.FindAction("Bump", throwIfNotFound: true);
        m_Player_Set = m_Player.FindAction("Set", throwIfNotFound: true);
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Serve;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_Spike;
    private readonly InputAction m_Player_Dig;
    private readonly InputAction m_Player_Bump;
    private readonly InputAction m_Player_Set;
    public struct PlayerActions
    {
        private @PlayerControls m_Wrapper;
        public PlayerActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Serve => m_Wrapper.m_Player_Serve;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @Spike => m_Wrapper.m_Player_Spike;
        public InputAction @Dig => m_Wrapper.m_Player_Dig;
        public InputAction @Bump => m_Wrapper.m_Player_Bump;
        public InputAction @Set => m_Wrapper.m_Player_Set;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Serve.started += instance.OnServe;
            @Serve.performed += instance.OnServe;
            @Serve.canceled += instance.OnServe;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Spike.started += instance.OnSpike;
            @Spike.performed += instance.OnSpike;
            @Spike.canceled += instance.OnSpike;
            @Dig.started += instance.OnDig;
            @Dig.performed += instance.OnDig;
            @Dig.canceled += instance.OnDig;
            @Bump.started += instance.OnBump;
            @Bump.performed += instance.OnBump;
            @Bump.canceled += instance.OnBump;
            @Set.started += instance.OnSet;
            @Set.performed += instance.OnSet;
            @Set.canceled += instance.OnSet;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Serve.started -= instance.OnServe;
            @Serve.performed -= instance.OnServe;
            @Serve.canceled -= instance.OnServe;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Spike.started -= instance.OnSpike;
            @Spike.performed -= instance.OnSpike;
            @Spike.canceled -= instance.OnSpike;
            @Dig.started -= instance.OnDig;
            @Dig.performed -= instance.OnDig;
            @Dig.canceled -= instance.OnDig;
            @Bump.started -= instance.OnBump;
            @Bump.performed -= instance.OnBump;
            @Bump.canceled -= instance.OnBump;
            @Set.started -= instance.OnSet;
            @Set.performed -= instance.OnSet;
            @Set.canceled -= instance.OnSet;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnServe(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnSpike(InputAction.CallbackContext context);
        void OnDig(InputAction.CallbackContext context);
        void OnBump(InputAction.CallbackContext context);
        void OnSet(InputAction.CallbackContext context);
    }
}
