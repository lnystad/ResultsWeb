<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
      xmlns:msxsl="urn:schemas-microsoft-com:xslt"
     exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/">
    <xsl:apply-templates select="/Merged/report" />
    <xsl:apply-templates select="/report" />
  </xsl:template>
  
  <xsl:template match="result">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="result/@ref">

  </xsl:template>

  <xsl:template match="result/@totsum">
  
       <xsl:variable name="TreffNoder">
         <TreffTotal>
            <xsl:choose>
             <xsl:when test="contains(., '/')">
               
                
                   <xsl:for-each select="../series">
                        <xsl:variable name="treff">
                         <xsl:value-of select="number(substring-before(@sum,'/'))"/> 
                        </xsl:variable>
                        <xsl:if test="$treff!='NaN'">
                        <Treff>
                           <xsl:value-of select="$treff"/> 
                        </Treff>
                        </xsl:if>
                   </xsl:for-each>
               
                 
             </xsl:when>
             <xsl:otherwise>
             </xsl:otherwise>
            </xsl:choose>
          </TreffTotal>
       </xsl:variable>  
     <xsl:variable name="InnerTreffNoder">
         <TreffTotal>
            <xsl:choose>
             <xsl:when test="contains(., '/')">
               
                
                   <xsl:for-each select="../series">
                      
                        <xsl:variable name="treff">
                         <xsl:value-of select="number(substring-after(@sum,'/'))"/> 
                        </xsl:variable>
                        <xsl:if test="$treff!='NaN'">
                        <Treff>
                           <xsl:value-of select="$treff"/> 
                        </Treff>
                        </xsl:if>
                   </xsl:for-each>
               
                 
             </xsl:when>
             <xsl:otherwise>
             </xsl:otherwise>
            </xsl:choose>
          </TreffTotal>
       </xsl:variable>  
       
       <xsl:variable name="TreffNoderNum" select="msxsl:node-set($TreffNoder)"/>

    
       <xsl:variable name="Totaltreff">
            <xsl:value-of select="sum($TreffNoderNum/*/*)"></xsl:value-of>
       </xsl:variable>
       <xsl:variable name="TotalInnertreff">
             <xsl:value-of select="sum(msxsl:node-set($InnerTreffNoder)/*/*)"></xsl:value-of>
       </xsl:variable>
 
       <xsl:attribute name="totsum">
           <xsl:choose>
              <xsl:when test="contains(., '/') and $Totaltreff and $TotalInnertreff">
                <xsl:value-of select="concat($Totaltreff,'/',$TotalInnertreff)"/>
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="."/>
              </xsl:otherwise>
           </xsl:choose>
           
          
        </xsl:attribute>
  </xsl:template>
    
  <xsl:template match="result[@ref]">

    <xsl:variable name ="Lagnr">
      <xsl:variable name="Startlagnr">
        <xsl:choose>
          <xsl:when test="/Merged/report/resulttotsum/@name = 'Omgang'">
            <xsl:value-of select="number(100)"/>
          </xsl:when>
          <xsl:when test="/Merged/report/resulttotsum/@name = 'Finale'">
            <xsl:value-of select="number(100)"/>
          </xsl:when>
          <xsl:when test="/Merged/report/resulttotsum/@name = 'Mesterskap'">
            <xsl:value-of select="number(100)"/>
          </xsl:when>
          <xsl:when test="/Merged/report/resulttotsum/@name = 'Lagskyting'">
            <xsl:value-of select="number(200)"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="number(0)"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:variable>

      <xsl:value-of select="$Startlagnr + number(substring-after(/Merged/report/header/@name,'Lag '))"/>
    </xsl:variable>

      <xsl:variable name ="serienavn">
         <xsl:value-of select="substring(/Merged/report/column/series/@name,1,5)"/>  
       </xsl:variable>
    
    <xsl:variable name ="MinneSkyting">
      <xsl:choose>
        <xsl:when test="$serienavn = 'Minne'">
            <xsl:value-of select="boolean(1)"/>  
        </xsl:when>
      </xsl:choose>
    </xsl:variable>
    
    <xsl:variable name ="skivenr">
      <xsl:value-of select="@num"/>
    </xsl:variable>

   <xsl:variable name ="serienr">
      <xsl:choose>
        <xsl:when test="substring(/Merged/report/column/series/@name,1,5) = 'Minne'">
        </xsl:when>
      </xsl:choose>
    </xsl:variable>

   <xsl:variable name ="filenameToFind">
      <xsl:choose>
        <xsl:when test="string($MinneSkyting) = 'true'">
          <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'-0','.PNG')"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'.PNG')"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:variable name ="filenameToFind2">
      <xsl:choose>
        <xsl:when test="string($MinneSkyting) = 'true'">
          <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'-0','.png')"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'.png')"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    

    <xsl:variable name="FoundBitmaps">
      <Hits>
        <xsl:for-each select="/Merged/BitmapDirInfo/Bitmaps/FileName" >
          <xsl:if test="current() = $filenameToFind or current() = $filenameToFind2">
            <Hit>
              <xsl:value-of select="current()"/>
            </Hit>
          </xsl:if>
        </xsl:for-each>
      </Hits>
    </xsl:variable>

    <xsl:variable name="FoundBitmap">
      <xsl:value-of select="msxsl:node-set($FoundBitmaps)/Hits/Hit"/>
    </xsl:variable>

    <xsl:copy>
      <!-- produce a fill attribute with content "red" -->
      <xsl:if test="string($FoundBitmap) !=''">
        <xsl:attribute name="ref">
          <xsl:value-of select="concat(/Merged/BitmapDirInfo/BitmapSubDir,'/',$FoundBitmap)"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

  <!-- this template is applied to a path node that doesn't have a fill attribute -->
  <xsl:template match="result[not(@ref)]">
    <!-- copy me and my attributes and my subnodes, applying templates as necessary, and add a fill attribute set to red -->
    <xsl:variable name ="Lagnr">
      <xsl:variable name="Startlagnr">
        <xsl:choose>
          <xsl:when test="/Merged/report/resulttotsum/@name = 'Omgang'">
            <xsl:value-of select="number(100)"/>
          </xsl:when>
          <xsl:when test="/Merged/report/resulttotsum/@name = 'Finale'">
            <xsl:value-of select="number(100)"/>
          </xsl:when>
          <xsl:when test="/Merged/report/resulttotsum/@name = 'Mesterskap'">
            <xsl:value-of select="number(100)"/>
          </xsl:when>
          <xsl:when test="/Merged/report/resulttotsum/@name = 'Lagskyting'">
            <xsl:value-of select="number(200)"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="number(0)"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:variable>

      <xsl:value-of select="$Startlagnr + number(substring-after(/Merged/report/header/@name,'Lag '))"/>
    </xsl:variable>

       <xsl:variable name ="serienavn">
         <xsl:value-of select="substring(/Merged/report/column/series/@name,1,5)"/>  
       </xsl:variable>
    
    <xsl:variable name ="MinneSkyting">
      <xsl:choose>
        <xsl:when test="$serienavn = 'Minne'">
            <xsl:value-of select="boolean(1)"/>  
        </xsl:when>
      </xsl:choose>
    </xsl:variable>

    <xsl:variable name ="skivenr">
      <xsl:value-of select="@num"/>
    </xsl:variable>


    <xsl:variable name ="filenameToFind">
      <xsl:choose>
        <xsl:when test="string($MinneSkyting) = 'true'">
          <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'-0','.PNG')"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'.PNG')"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:variable name ="filenameToFind2">
      <xsl:choose>
        <xsl:when test="string($MinneSkyting) = 'true'">
          <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'-0','.png')"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'.png')"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>


    <xsl:variable name="FoundBitmaps">
      <Hits>
        <xsl:for-each select="/Merged/BitmapDirInfo/Bitmaps/FileName" >
          <xsl:if test="current() = $filenameToFind or current() = $filenameToFind2">
            <Hit>
              <xsl:value-of select="current()"/>
            </Hit>
          </xsl:if>
        </xsl:for-each>
      </Hits>
    </xsl:variable>

    <xsl:variable name="FoundBitmap">
      <xsl:value-of select="msxsl:node-set($FoundBitmaps)/Hits/Hit"/>
    </xsl:variable>


    <xsl:copy>
      <xsl:if test="string($FoundBitmap) !=''">
        <xsl:attribute name="ref">
          <xsl:value-of select="concat(/Merged/BitmapDirInfo/BitmapSubDir,'/',$FoundBitmap)"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select="@*|node()"/>

    </xsl:copy>
  </xsl:template>

  <!-- FELT-->

  <xsl:template match="result/series[@ref]">

    <xsl:variable name ="Lagnr">
      <xsl:variable name="Startlagnr">
        <xsl:choose>
          <xsl:when test="/Merged/report/resulttotsum/@name = 'Omgang'">
            <xsl:value-of select="number(100)"/>
          </xsl:when>
          <xsl:when test="/Merged/report/resulttotsum/@name = 'Finale'">
            <xsl:value-of select="number(100)"/>
          </xsl:when>
          <xsl:when test="/Merged/report/resulttotsum/@name = 'Mesterskap'">
            <xsl:value-of select="number(100)"/>
          </xsl:when>
          <xsl:when test="/Merged/report/resulttotsum/@name = 'Lagskyting'">
            <xsl:value-of select="number(200)"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="number(0)"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:variable>

      <xsl:value-of select="$Startlagnr + number(substring-after(/Merged/report/header/@name,'Lag '))"/>
    </xsl:variable>

   <xsl:variable name ="serienavn">
         <xsl:value-of select="substring(/Merged/report/column/series/@name,1,5)"/>  
       </xsl:variable>
    
    <xsl:variable name ="MinneSkyting">
      <xsl:choose>
        <xsl:when test="$serienavn = 'Minne'">
            <xsl:value-of select="boolean(1)"/>  
        </xsl:when>
      </xsl:choose>
    </xsl:variable>
    
      
    <xsl:variable name ="skivenr">
      <xsl:value-of select="ancestor::result[1]/@num"/>
    </xsl:variable>

    <xsl:variable name ="serienr">
      <xsl:value-of select="@id"/>
    </xsl:variable>

    <xsl:variable name ="filenameToFind">
      <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'-',$serienr,'.PNG')"/>
    </xsl:variable>
    <xsl:variable name ="filenameToFind2">
      <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'-',$serienr,'.png')"/>
    </xsl:variable>

    <xsl:variable name="FoundBitmaps">
      <Hits>
        <xsl:for-each select="/Merged/BitmapDirInfo/Bitmaps/FileName" >
          <xsl:if test="current() = $filenameToFind or current() = $filenameToFind2">
            <Hit>
              <xsl:value-of select="current()"/>
            </Hit>
          </xsl:if>
        </xsl:for-each>
      </Hits>
    </xsl:variable>

    <xsl:variable name="FoundBitmap">
      <xsl:value-of select="msxsl:node-set($FoundBitmaps)/Hits/Hit"/>
    </xsl:variable>

    <xsl:copy>
      <!-- produce a fill attribute with content "red" -->
      <xsl:if test="string($FoundBitmap) !='' and string($MinneSkyting) != 'true'">
        <xsl:attribute name="ref">
          <xsl:value-of select="concat(/Merged/BitmapDirInfo/BitmapSubDir,'/',$FoundBitmap)"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

  <!-- this template is applied to a path node that doesn't have a fill attribute -->
  <xsl:template match="result/series[not(@ref)]">
    <!-- copy me and my attributes and my subnodes, applying templates as necessary, and add a fill attribute set to red -->
    <xsl:variable name ="Lagnr">
      <xsl:variable name="Startlagnr">
        <xsl:choose>
          <xsl:when test="/Merged/report/resulttotsum/@name = 'Omgang'">
            <xsl:value-of select="number(100)"/>
          </xsl:when>
            <xsl:when test="/Merged/report/resulttotsum/@name = 'Finale'">
            <xsl:value-of select="number(100)"/>
          </xsl:when>
          <xsl:when test="/Merged/report/resulttotsum/@name = 'Mesterskap'">
            <xsl:value-of select="number(100)"/>
          </xsl:when>
          <xsl:when test="/Merged/report/resulttotsum/@name = 'Lagskyting'">
            <xsl:value-of select="number(200)"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="number(0)"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:variable>

      <xsl:value-of select="$Startlagnr + number(substring-after(/Merged/report/header/@name,'Lag '))"/>
    </xsl:variable>

    <xsl:variable name ="serienavn">
         <xsl:value-of select="substring(/Merged/report/column/series/@name,1,5)"/>  
       </xsl:variable>
    
    <xsl:variable name ="MinneSkyting">
      <xsl:choose>
        <xsl:when test="$serienavn = 'Minne'">
            <xsl:value-of select="boolean(1)"/>  
        </xsl:when>
      </xsl:choose>
    </xsl:variable>
      
    <xsl:variable name ="skivenr">
      <xsl:value-of select="ancestor::result[1]/@num"/>
    </xsl:variable>

    <xsl:variable name ="serienr">
      <xsl:value-of select="@id"/>
    </xsl:variable>

    <xsl:variable name ="filenameToFind">
      <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'-',$serienr,'.PNG')"/>
    </xsl:variable>

    <xsl:variable name ="filenameToFind2">
      <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'-',$serienr,'.png')"/>
    </xsl:variable>

    <xsl:variable name="FoundBitmaps">
      <Hits>
        <xsl:for-each select="/Merged/BitmapDirInfo/Bitmaps/FileName" >
          <xsl:if test="current() = $filenameToFind or current() = $filenameToFind2">
            <Hit>
              <xsl:value-of select="current()"/>
            </Hit>
          </xsl:if>
        </xsl:for-each>
      </Hits>
    </xsl:variable>

    <xsl:variable name="FoundBitmap">
      <xsl:value-of select="msxsl:node-set($FoundBitmaps)/Hits/Hit"/>
    </xsl:variable>


    <xsl:copy>
      <xsl:if test="string($FoundBitmap) !='' and string($MinneSkyting) != 'true'">
        <xsl:attribute name="ref">
          <xsl:value-of select="concat(/Merged/BitmapDirInfo/BitmapSubDir,'/',$FoundBitmap)"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select="@*|node()"/>

    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
