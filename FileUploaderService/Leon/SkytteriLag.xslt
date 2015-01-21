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
    <Skyttere>
      <xsl:apply-templates select="/Merged/report" />
    </Skyttere>
  </xsl:template>
  <xsl:template match="report">
    <xsl:apply-templates select="/Merged/report/data/result" />
  </xsl:template>
  
  <xsl:template match="result">
    <xsl:variable name ="Lagnr">
      <xsl:value-of select="substring-after(/Merged/report/header/@name,'Lag ')"/>
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

    <!-- produce a fill attribute with content "red" -->
    <xsl:if test="string($FoundBitmap) !=''">
      <xsl:element name="Skytter">
        <xsl:attribute name="name">
          <xsl:value-of select="@name"/>
        </xsl:attribute>
        <xsl:attribute name="club">
          <xsl:value-of select="@club"/>
        </xsl:attribute>
        <xsl:attribute name="class">
          <xsl:value-of select="@class"/>
        </xsl:attribute>
        <xsl:attribute name="ref">
          <xsl:value-of select="concat(/Merged/BitmapDirInfo/BitmapSubDir,'/',$FoundBitmap)"/>
        </xsl:attribute>
      </xsl:element>
    </xsl:if>


  </xsl:template>




</xsl:stylesheet>
