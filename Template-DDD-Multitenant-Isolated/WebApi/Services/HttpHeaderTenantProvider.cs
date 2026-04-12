using SharedKernel.Ports.Out.MultiTenancy;

namespace WebApi.Services;

public class HttpHeaderTenantProvider(IHttpContextAccessor accessor) : ITenantProvider
{
    private const string TenantHeader = "X-Tenant-ID";
    private string? _manualTenantId;

    public string GetTenantId()
    {
        if (!string.IsNullOrEmpty(_manualTenantId)) return _manualTenantId;
        
        var context = accessor.HttpContext;
        if (context == null) return string.Empty;

        if (context.Request.Headers.TryGetValue(TenantHeader, out var tenantId))
        {
            var tenantIdValue = tenantId.ToString().Trim();
            
            if (string.IsNullOrWhiteSpace(tenantIdValue))
                throw new BadHttpRequestException($"El header {TenantHeader} no puede estar vacío.");
            
            if (!IsValidTenantId(tenantIdValue))
                throw new BadHttpRequestException($"El tenant '{tenantIdValue}' contiene caracteres inválidos.");
            
            return tenantIdValue;
        }

        throw new BadHttpRequestException($"El header {TenantHeader} es obligatorio.");
    }

    public void SetTenantId(string tenantId) => _manualTenantId = tenantId;
    
    private static bool IsValidTenantId(string tenantId)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(tenantId, @"^[a-zA-Z0-9_\-]*$");
    }
}
