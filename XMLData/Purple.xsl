<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/StudentsList">
        <html>
            <head>
                <style>
                    table {
                    font-family: Arial, sans-serif;
                    border-collapse: collapse;
                    width: 100%;
                    }

                    th, td {
                    border: 1px solid #dddddd;
                    text-align: left;
                    padding: 8px;
                    }

                    th {
                    background-color: #9900ff;
                    }
                </style>
            </head>
            <body>
                <h2>Students List</h2>
                <table>
                    <tr>
                        <th>First Name</th>
                        <th>Last Name</th>
                        <th>Faculty</th>
                        <th>Department</th>
                        <th>Course</th>
                        <th>Room Number</th>
                        <th>Check In Date</th>
                        <th>Check Out Date</th>
                    </tr>
                    <xsl:apply-templates select="Student" />
                </table>
            </body>
        </html>
    </xsl:template>

    <xsl:template match="Student">
        <tr>
            <td><xsl:value-of select="FirstName" /></td>
            <td><xsl:value-of select="LastName" /></td>
            <td><xsl:value-of select="Faculty" /></td>
            <td><xsl:value-of select="Department" /></td>
            <td><xsl:value-of select="Course" /></td>
            <td><xsl:value-of select="DormAttributes/RoomNumber" /></td>
            <td><xsl:value-of select="DormAttributes/CheckInDate" /></td>
            <td><xsl:value-of select="DormAttributes/CheckOutDate" /></td>
        </tr>
    </xsl:template>
</xsl:stylesheet>
