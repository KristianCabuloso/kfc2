using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerController : EntityBehaviour<ITestState>
{
    public float moveSpeed = 3f;

    // Similar ao Start(), mas s� ocorre quando a entidade � atrelada ao objeto
    public override void Attached()
    {
        // Fazer o transform deste jogador ser controlado pelo transform do Photon.
        // state.Transform: Transform que foi criado pelo editor de assets do Photon,
        // poderia receber um nome personalizado al�.
        state.SetTransforms(state.Transform, transform);
    }

    // Similar ao Update(), mas do pr�prio Photon E ocorre apenas no PC do dono deste objeto
    public override void SimulateOwner()
    {
        // Movimenta��o bem toscona, apenas para testes
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        // Note que o Bolt possui seu pr�prio deltatime
        transform.position += movement.normalized * moveSpeed * BoltNetwork.FrameDeltaTime;
    }
}
