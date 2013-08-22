<div id="nav">

	<!--[if gte IE 5]><![if lt IE 7]>
<style type="text/css">
.navmenu li
{
  float: left;
  width: 8em;
  font-family:arial,helvetica,tahoma,verdana,sans-serif;
  font-size:9px;
  line-height:12px;
}
</style>
<![endif]><![endif]--><attach event="onmouseover" handler="mouseover" />
<attach event="onmouseout" handler="mouseout" />
<script type="text/javascript">
function mouseover()
{
  element.className += ' hover';
  for(var x=0;x!=element.childNodes.length;++x)
  {
    if(element.childNodes[x].nodeType==1)
    {
      element.childNodes[x].className += 
        ' parent_hover';
    }
  }
}

function mouseout()
{
  element.className =
    element.className.replace(/ ?hover$/,'');
  for(var x=0;x!=element.childNodes.length;++x)
  {
    if(element.childNodes[x].nodeType==1)
    {
      element.childNodes[x].className =
      element.childNodes[x].className.replace
        (/ ?parent_hover$/,'');
    }
  }
}
</script>
<ul class="navmenu">
  <li><a href="index.php">Home</a></li>
  <li><a href="about.php">About</a>
  <li><a href="load_steps.php">How To</a><ul>
    <li><a href="load_steps.php">Export Access Table</a></li>
    <li><a href="upload_steps.php">Upload Data Table&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</a></li>
    </ul></li>
  <li><a href="load_t.php">Load Table</a>

  <li><a href="select_tables.php">Data</a>
  <li><a href="watershed/Final/index.html">Watershed</a><ul>
  <!-- <li><a href="/tl2/item3"> * - * </a><ul>
    <li><a href="/tl2/item3/one"> - *</a></li>
    <li><a href="/tl2/item3/two"> - *</a></li>
    <li><a href="/tl2/item3/three"> - *</a></li>
    </ul></li>
   <li><a href="/tl2/item4"> * </a></li>
  </ul></li>
  <li><a href="/tl3">Info</a>
   <!--<li><span> * - * </span><ul>
    <li><a href="/tl3/item1/one"> - * </a></li>
    <li><a href="/tl3/item1/two"> - * </a></li>
    <li><a href="/tl3/item1/three"> - * </a></li>
    </ul></li>
   <li><a href="/tl3/item2"> * - * </a></li>
  </ul>--></li>
  
 </ul>
	<!--<a href="index.php">Home</a>
	<a href="about.php">How To</a>-->
	
</div> <!-- end #nav -->
