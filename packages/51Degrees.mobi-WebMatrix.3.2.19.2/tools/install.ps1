param($installPath, $toolsPath, $package, $project)



try
{
  $url = "http://51degrees.github.io/dotNET-Device-Detection/Release3.2.19.html" 
  $dte2 = Get-Interface $dte ([EnvDTE80.DTE2])

  $dte2.ItemOperations.Navigate($url) 
 
}
catch
{
 
# nothing will be displayed

}
