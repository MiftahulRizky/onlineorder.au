Imports System.Data.SqlClient

Partial Class Setting_Price_Surcharge_Add
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/price/surcharge", False)
            Exit Sub
        End If

        If Not Session("LevelName") = "Leader" Then
            Response.Redirect("~/setting/price/surcharge", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            BackColor()

            BindDesignType()
            BindBlindType(ddlDesign.SelectedValue)
            BindFieldName()
        End If
    End Sub

    Protected Sub ddlDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindBlindType(ddlDesign.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        BackColor()
        Try
            If msgError.InnerText = "" Then
                Dim designType As String = UCase(ddlDesign.SelectedValue).ToString()
                Dim blindType As String = UCase(ddlBlind.SelectedValue).ToString()
                Dim finalFormula As String = ddlFieldName.SelectedValue & txtFormula.Text.Trim()

                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Dim thisId As String = String.Empty
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO PriceSurcharges OUTPUT INSERTED.Id VALUES (NEWID(), @DesignId, @BlindId, @BlindNumber, @Name, @FieldName, @Formula, @Charge, @Description, @Active)")
                        myCmd.Parameters.AddWithValue("@DesignId", designType)
                        myCmd.Parameters.AddWithValue("@BlindId", blindType)
                        myCmd.Parameters.AddWithValue("@BlindNumber", ddlBlindNumber.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@FieldName", ddlFieldName.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Formula", finalFormula)
                        myCmd.Parameters.AddWithValue("@Charge", txtCharge.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Description", descText)
                        myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        thisId = myCmd.ExecuteScalar().ToString()
                        thisConn.Close()
                    End Using
                End Using

                Dim dataLog As Object() = {"PriceSurcharges", thisId, Session("LoginId").ToString(), "Price Surcharge Created"}
                settingCfg.Log_Price(dataLog)

                Response.Redirect("~/setting/price/surcharge", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/surcharge", False)
    End Sub

    Private Sub BindDesignType()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = settingCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Designs WHERE Active = 1 ORDER BY Name ASC")
            ddlDesign.DataTextField = "NameText"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            If ddlDesign.Items.Count > 0 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindBlindType(DesignId As String)
        ddlBlind.Items.Clear()
        Try
            If Not DesignId = "" Then
                ddlBlind.DataSource = settingCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Blinds WHERE DesignId = '" + DesignId + "' AND Active = 1 ORDER BY Name ASC")
                ddlBlind.DataTextField = "NameText"
                ddlBlind.DataValueField = "Id"
                ddlBlind.DataBind()

                If ddlBlind.Items.Count > 0 Then
                    ddlBlind.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindFieldName()
        ddlFieldName.Items.Clear()
        Try
            ddlFieldName.DataSource = settingCfg.GetListData("SELECT COLUMN_NAME AS FieldName FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'viewDetails'")
            ddlFieldName.DataTextField = "FieldName"
            ddlFieldName.DataValueField = "FieldName"
            ddlFieldName.DataBind()

            If ddlFieldName.Items.Count > 0 Then
                ddlFieldName.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        MessageError(False, String.Empty)

        ddlDesign.BackColor = Drawing.Color.Empty
        ddlBlind.BackColor = Drawing.Color.Empty
        txtName.BackColor = Drawing.Color.Empty
        ddlFieldName.BackColor = Drawing.Color.Empty
        txtFormula.BackColor = Drawing.Color.Empty
        txtCharge.BackColor = Drawing.Color.Empty
        txtDescription.BackColor = Drawing.Color.Empty
        ddlActive.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub
End Class
