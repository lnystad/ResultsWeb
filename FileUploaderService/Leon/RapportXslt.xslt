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
  </xsl:template>

  <xsl:template match="result">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="result/@ref">

  </xsl:template>
  
  <xsl:template match="result[@ref]">
    
    <xsl:variable name ="Lagnr">
      <xsl:variable name="Startlagnr">
        <xsl:choose>
          <xsl:when test="/Merged/report/resulttotsum/@name = 'Omgang'">
            <xsl:value-of select="number(100)"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="number(0)"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:variable>
    
      <xsl:value-of select="$Startlagnr + number(substring-after(/Merged/report/header/@name,'Lag '))"/>
    </xsl:variable>

    <xsl:variable name ="skivenr">
      <xsl:value-of select="@num"/>
    </xsl:variable>

    <xsl:variable name ="filenameToFind">
      <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'.PNG')"/>
    </xsl:variable>

    <xsl:variable name="FoundBitmaps">
      <Hits>
        <xsl:for-each select="/Merged/BitmapDirInfo/Bitmaps/FileName" >
          <xsl:if test="current() = $filenameToFind">
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
          <xsl:otherwise>
            <xsl:value-of select="number(0)"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:variable>

      <xsl:value-of select="$Startlagnr + number(substring-after(/Merged/report/header/@name,'Lag '))"/>
    </xsl:variable>


    <xsl:variable name ="skivenr">
      <xsl:value-of select="@num"/>
    </xsl:variable>

    <xsl:variable name ="filenameToFind">
      <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'.PNG')"/>
    </xsl:variable>

    <xsl:variable name="FoundBitmaps">
      <Hits>
        <xsl:for-each select="/Merged/BitmapDirInfo/Bitmaps/FileName" >
          <xsl:if test="current() = $filenameToFind">
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



</xsl:stylesheet>
