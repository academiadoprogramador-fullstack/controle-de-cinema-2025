﻿namespace ControleDeCinema.Dominio.ModuloAutenticacao;

public interface ITenantProvider
{
    Guid? UsuarioId { get; }
}