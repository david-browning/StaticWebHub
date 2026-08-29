param(
    [string]$BaseUrl = 'http://localhost:7071/api'
)

$ErrorActionPreference = 'Stop'

$tests = @(
    @{
        Name  = 'Test'
        Route = 'test'
        Body  = @{
            message = 'Hello from PowerShell'
        }
    },
    @{
        Name  = 'JsonRequest'
        Route = 'jsonrequest'
        Body  = @{
            projectName = 'Test Project'
            teamSize    = 5
            category    = 'research'
            enabled     = $true
        }
    }
)

foreach ($test in $tests) {
    $uri = "$($BaseUrl.TrimEnd('/'))/$($test.Route)"
    $json = $test.Body | ConvertTo-Json -Depth 10

    Write-Host
    Write-Host "Testing $($test.Name)"
    Write-Host "POST $uri"
    Write-Host
    Write-Host "Request:"
    Write-Host $json
    Write-Host

    try {
        $response = Invoke-RestMethod -Uri $uri -Method Post -ContentType 'application/json' -Body $json
        Write-Host "Response:"
        $response | ConvertTo-Json -Depth 10 | Write-Host

        Write-Host
        Write-Host -ForegroundColor Green "PASS"
    }
    catch {
        Write-Host
        Write-Host -ForegroundColor Red "FAIL"
        Write-Host $_.Exception.Message
    }
}