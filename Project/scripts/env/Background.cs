using Godot;
using System;
using System.Collections.Generic;

public class Background : ParallaxBackground
{
    [Export] private bool _canProcess = true; // Nomenclatura mais clara para variáveis privadas
    [Export] private List<int> _layerSpeeds = new List<int>(); // Usando List<float> ao invés de Array para maior controle e segurança

    private List<ParallaxLayer> _parallaxLayers = new List<ParallaxLayer>();

    public override void _Ready()
    {
        // Inicializa a lista de camadas ParallaxLayer
        foreach (var child in GetChildren())
        {
            if (child is ParallaxLayer layer)
            {
                _parallaxLayers.Add(layer);
            }
        }

        // Ativa ou desativa o processamento de física com base no valor de _canProcess
        SetPhysicsProcess(_canProcess);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_parallaxLayers.Count == 0 || _layerSpeeds.Count != _parallaxLayers.Count)
        {
            GD.PrintErr("Erro: A quantidade de velocidades não corresponde à quantidade de camadas.");
            return; // Evita exceções em tempo de execução
        }

        for (int i = 0; i < _parallaxLayers.Count; i++)
        {
            var layer = _parallaxLayers[i];
            layer.MotionOffset += new Vector2(_layerSpeeds[i] * delta, 0); // Ajusta o deslocamento da camada
        }
    }
}