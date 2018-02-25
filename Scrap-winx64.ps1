$array = new-object System.Collections.ArrayList

DO
{
write-host "Do you want to add Google Search Link ?"
$link =  read-host
if ($link) { $array.Add($link) }
} While ($link)

dotnet.exe .\Silashu\bin\Release\PublishOutput\EmailScraper.dll $array