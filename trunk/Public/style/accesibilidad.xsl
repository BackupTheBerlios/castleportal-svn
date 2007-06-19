<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="xml">
		<xsl:apply-templates />
	</xsl:template>
	<xsl:template match="img">
		(Imagen <xsl:value-of select="@alt"/>)<br/>
	</xsl:template>
	<xsl:template match="object">
		(OBJETO <xsl:value-of select="@alt"/>)<br/>
	</xsl:template>
	<xsl:template match="embed">
		(OBJETO <xsl:value-of select="@alt"/>)<br/>
	</xsl:template>
</xsl:stylesheet>
