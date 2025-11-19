Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Surcharge_Detail
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/price/surcharge", False)
            Exit Sub
        End If

        If Session("surchargeDetail") = "" Then
            Response.Redirect("~/setting/price/surcharge", False)
            Exit Sub
        End If

        lblId.Text = UCase(Session("surchargeDetail")).ToString()
        If Not IsPostBack Then
            BackColor()
            BindData(lblId.Text)
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

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE PriceSurcharges SET DesignId=@DesignId, BlindId=@BlindId, BlindNumber=@BlindNumber, Name=@Name, FieldName=@FieldName, Formula=@Formula, Charge=@Charge, Description=@Description, Active=@Active WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", lblId.Text)
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
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                Dim dataLog As Object() = {"Surcharges", lblId.Text, Session("LoginId").ToString(), "Price Surcharge Updated"}
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

    Private Sub BindData(Id As String)
        Try
            Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM PriceSurcharges WHERE Id = '" + Id + "'")

            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/setting/price/surcharge", False)
                Exit Sub
            End If

            Dim designId As String = myData.Tables(0).Rows(0).Item("DesignId").ToString()

            BindDesignType()
            BindBlindType(designId)
            BindFieldName()

            ddlDesign.SelectedValue = designId
            ddlBlind.SelectedValue = myData.Tables(0).Rows(0).Item("BlindId").ToString()
            ddlBlindNumber.SelectedValue = myData.Tables(0).Rows(0).Item("BlindNumber").ToString()
            txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
            ddlFieldName.SelectedValue = myData.Tables(0).Rows(0).Item("FieldName").ToString()
            txtFormula.Text = myData.Tables(0).Rows(0).Item("Formula").ToString().Replace(ddlFieldName.SelectedValue, "")
            txtCharge.Text = myData.Tables(0).Rows(0).Item("Charge").ToString()
            txtDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
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
