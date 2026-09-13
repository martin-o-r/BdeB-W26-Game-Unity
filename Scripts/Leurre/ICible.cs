using System;
using UnityEngine;

/// <summary>
/// Cette interface represente une cible qui peut etre attaquee par les ennemis
/// </summary>
/// <remarks>
/// Dans notre cas, cette interface est implemente par Olivia et le leurre.
/// Les ennemis poursuivront/attaqueront une "cible".
/// Les ennemis peuvent savoir ou les cibles sont grace a l'attribut "Position"
/// </remarks>

public interface ICible
{
    Vector3 Position { get; }

    void SeFaireAttaquer(int dommage);

    event Action OnCibleDetruite;
}
