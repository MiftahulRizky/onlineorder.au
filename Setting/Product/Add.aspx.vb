Imports System.Data.SqlClient

Partial Class Setting_Product_Add
    Inherits Page

    Dim settingCfg As New SettingConfig

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/", False)
            Exit Sub
        End If

        If Not Session("LevelName") = "Leader" Then
            Response.Redirect("~/setting/product", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            BackColor()

            BindDesign()
            BindBlind(ddlDesignId.SelectedValue)
            BindTubeType()
            BindControlType()
            BindColourType()
        End If
    End Sub

    Protected Sub ddlDesignId_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindBlind(ddlDesignId.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        BackColor()
        Try
            If ddlDesignId.SelectedValue = "" Then
                MessageError(True, "DESIGN NAME IS REQUIRED !")
                ddlDesignId.BackColor = Drawing.Color.Red
                ddlDesignId.Focus()
                Exit Sub
            End If

            If ddlBlindId.SelectedValue = "" Then
                MessageError(True, "BLIND NAME IS REQUIRED !")
                ddlBlindId.BackColor = Drawing.Color.Red
                ddlBlindId.Focus()
                Exit Sub
            End If

            If txtName.Text = "" Then
                MessageError(True, "PRODUCT NAME IS REQUIRED !")
                txtName.BackColor = Drawing.Color.Red
                txtName.Focus()
                Exit Sub
            End If

            If ddlTubeType.SelectedValue = "" Then
                MessageError(True, "TUBE TYPE IS REQUIRED !")
                ddlTubeType.BackColor = Drawing.Color.Red
                ddlTubeType.Focus()
                Exit Sub
            End If

            If ddlControlType.SelectedValue = "" Then
                MessageError(True, "CONTROL TYPE IS REQUIRED !")
                ddlControlType.BackColor = Drawing.Color.Red
                ddlControlType.Focus()
                Exit Sub
            End If

            If ddlColourType.SelectedValue = "" Then
                MessageError(True, "COLOUR TYPE IS REQUIRED !")
                ddlColourType.BackColor = Drawing.Color.Red
                ddlColourType.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim designType As String = UCase(ddlDesignId.SelectedValue).ToString()
                Dim blindType As String = UCase(ddlBlindId.SelectedValue).ToString()
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Dim thisId As String = String.Empty
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Products OUTPUT INSERTED.Id VALUES (NEWID(), @DesignId, @BlindId, @Name, @TubeType, @ControlType, @ColourType, @Description, @Active)")
                        myCmd.Parameters.AddWithValue("@DesignId", designType)
                        myCmd.Parameters.AddWithValue("@BlindId", blindType)
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@TubeType", ddlTubeType.SelectedValue)
                        myCmd.Parameters.AddWithValue("@ControlType", ddlControlType.SelectedValue)
                        myCmd.Parameters.AddWithValue("@ColourType", ddlColourType.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Description", descText)
                        myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        thisId = myCmd.ExecuteScalar().ToString()
                        thisConn.Close()
                    End Using
                End Using

                Dim dataLog As Object() = {"Products", thisId, Session("LoginId").ToString(), "Product Created"}
                settingCfg.Log_Product(dataLog)

                Response.Redirect("~/setting/product", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSubmitTube_Click(sender As Object, e As EventArgs)
        BackColor()
        Try
            If Not txtTubeName.Text = "" Then
                Dim thisId As String = String.Empty
                Dim descText As String = txtTubeDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO ProductTubes OUTPUT INSERTED.Id VALUES (NEWID(), @Name, @Description, @Active)")
                        myCmd.Parameters.AddWithValue("@Name", txtTubeName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Description", descText)
                        myCmd.Parameters.AddWithValue("@Active", ddlTubeActive.SelectedValue)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        thisId = myCmd.ExecuteScalar().ToString()
                        thisConn.Close()
                    End Using
                End Using

                Dim dataLog As Object() = {"ProductTubes", thisId, Session("LoginId").ToString(), "Tube Type Created"}
                settingCfg.Log_Product(dataLog)

                BindTubeType()
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSubmitControl_Click(sender As Object, e As EventArgs)
        BackColor()
        Try
            If Not txtControlName.Text = "" Then

                Dim thisId As String = String.Empty
                Dim descText As String = txtControlDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO ProductControls OUTPUT INSERTED.Id VALUES (NEWID(), @Name, @Description, @Active)")
                        myCmd.Parameters.AddWithValue("@Name", txtControlName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Description", descText)
                        myCmd.Parameters.AddWithValue("@Active", ddlControlActive.SelectedValue)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        thisId = myCmd.ExecuteScalar().ToString()
                        thisConn.Close()
                    End Using
                End Using

                Dim dataLog As Object() = {"ProductTubes", thisId, Session("LoginId").ToString(), "Control Type Created"}
                settingCfg.Log_Product(dataLog)

                BindControlType()
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSubmitColour_Click(sender As Object, e As EventArgs)
        BackColor()
        Try
            If Not txtColourName.Text = "" Then
                Dim thisId As String = String.Empty
                Dim descText As String = txtColourDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO ProductColours OUTPUT INSERTED.Id VALUES (NEWID(), @Name, @Description, @Active)")
                        myCmd.Parameters.AddWithValue("@Name", txtColourName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Description", descText)
                        myCmd.Parameters.AddWithValue("@Active", ddlColourActive.SelectedValue)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        thisId = myCmd.ExecuteScalar().ToString()
                        thisConn.Close()
                    End Using
                End Using

                Dim dataLog As Object() = {"ProductColours", thisId, Session("LoginId").ToString(), "Colour Type Created"}
                settingCfg.Log_Product(dataLog)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/product/", False)
    End Sub

    Private Sub BindDesign()
        ddlDesignId.Items.Clear()
        Try
            ddlDesignId.DataSource = settingCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Designs WHERE Active=1 ORDER BY Name ASC")
            ddlDesignId.DataTextField = "NameText"
            ddlDesignId.DataValueField = "Id"
            ddlDesignId.DataBind()
            ddlDesignId.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindBlind(DesignId As String)
        ddlBlindId.Items.Clear()
        Try
            If Not DesignId = "" Then
                ddlBlindId.DataSource = settingCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Blinds WHERE DesignId='" + UCase(DesignId).ToString() + "' ORDER BY Name ASC")
                ddlBlindId.DataTextField = "NameText"
                ddlBlindId.DataValueField = "Id"
                ddlBlindId.DataBind()
                If ddlBlindId.Items.Count > 1 Then
                    ddlBlindId.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindTubeType()
        ddlTubeType.Items.Clear()
        Try
            ddlTubeType.DataSource = settingCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM ProductTubes ORDER BY Name ASC")
            ddlTubeType.DataTextField = "NameText"
            ddlTubeType.DataValueField = "Name"
            ddlTubeType.DataBind()
            If ddlTubeType.Items.Count > 1 Then
                ddlTubeType.Items.Insert(0, New ListItem("N/A", "N/A"))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindControlType()
        ddlControlType.Items.Clear()
        Try
            ddlControlType.DataSource = settingCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM ProductControls ORDER BY Name ASC")
            ddlControlType.DataTextField = "NameText"
            ddlControlType.DataValueField = "Name"
            ddlControlType.DataBind()
            If ddlControlType.Items.Count > 1 Then
                ddlControlType.Items.Insert(0, New ListItem("N/A", "N/A"))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindColourType()
        ddlColourType.Items.Clear()
        Try
            ddlColourType.DataSource = settingCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM ProductColours ORDER BY Name ASC")
            ddlColourType.DataTextField = "NameText"
            ddlColourType.DataValueField = "Name"
            ddlColourType.DataBind()

            If ddlColourType.Items.Count > 1 Then
                ddlColourType.Items.Insert(0, New ListItem("N/A", "N/A"))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        MessageError(False, "")

        ddlDesignId.BackColor = Drawing.Color.Empty
        ddlBlindId.BackColor = Drawing.Color.Empty
        txtName.BackColor = Drawing.Color.Empty
        ddlTubeType.BackColor = Drawing.Color.Empty
        ddlControlType.BackColor = Drawing.Color.Empty
        ddlColourType.BackColor = Drawing.Color.Empty
        txtDescription.BackColor = Drawing.Color.Empty
        ddlActive.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub
End Class
