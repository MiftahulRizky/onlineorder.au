Imports System.Data

Partial Class Setting_Fabric_Detail
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Session("fabricDetail") = "" Then
            Response.Redirect("~/setting/fabric", False)
            Exit Sub
        End If

        lblFabricIdOri.Text = Session("fabricDetail")
        If Not IsPostBack Then
            Call BackColor()
            Call BindDataFabric(lblFabricIdOri.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If ddlDesignId.Text = "" Then
                Call MessageError(True, "DESIGN TYPE IS REQUIRED !")
                ddlDesignId.BackColor = Drawing.Color.Red
                ddlDesignId.Focus()
                Exit Sub
            End If

            If txtId.Text = "" Then
                Call MessageError(True, "FABRIC ID IS REQUIRED !")
                txtId.BackColor = Drawing.Color.Red
                txtId.Focus()
                Exit Sub
            End If

            If Not txtId.Text = lblFabricIdOri.Text Then
                If txtId.Text = publicCfg.GetItemData("SELECT Id FROM Fabrics WHERE Id='" + txtId.Text + "'") Then
                    Call MessageError(True, "FABRIC ID ALEREADY EXISTS !")
                    txtId.BackColor = Drawing.Color.Red
                    txtId.Focus()
                    Exit Sub
                End If
            End If

            If txtName.Text = "" Then
                Call MessageError(True, "FABRIC NAME IS REQUIRED !")
                txtName.BackColor = Drawing.Color.Red
                txtName.Focus()
                Exit Sub
            End If

            If txtType.Text = "" Then
                Call MessageError(True, "FABRIC TYPE IS REQUIRED !")
                txtType.BackColor = Drawing.Color.Red
                txtType.Focus()
                Exit Sub
            End If

            If txtColour.Text = "" Then
                Call MessageError(True, "FABRIC COLOUR IS REQUIRED !")
                txtColour.BackColor = Drawing.Color.Red
                txtColour.Focus()
                Exit Sub
            End If

            If txtWidth.Text = "" Then
                Call MessageError(True, "FABRIC WIDTH IS REQUIRED !")
                txtWidth.BackColor = Drawing.Color.Red
                txtWidth.Focus()
                Exit Sub
            End If

            If txtGroup.Text = "" Then
                Call MessageError(True, "FABRIC GROUP IS REQUIRED !")
                txtGroup.BackColor = Drawing.Color.Red
                txtGroup.Focus()
                Exit Sub
            End If

            If ddlActive.Text = "" Then
                Call MessageError(True, "FABRIC ACTIVE IS REQUIRED !")
                ddlActive.BackColor = Drawing.Color.Red
                ddlActive.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                sdsPage.Update()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "UPDATE FABRIC. ID : " & txtId.Text)

                Response.Redirect("~/setting/fabric", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnSubmit_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/fabric", False)
    End Sub

    Private Sub BindDataFabric(Id As String)
        Try
            Call BindDataDesign()

            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM Fabrics WHERE Id = '" + Id + "'")
            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/setting/fabric", False)
                Exit Sub
            End If

            ddlDesignId.SelectedValue = myData.Tables(0).Rows(0).Item("DesignId").ToString()
            txtId.Text = myData.Tables(0).Rows(0).Item("Id").ToString()
            txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
            txtType.Text = myData.Tables(0).Rows(0).Item("Type").ToString()
            txtColour.Text = myData.Tables(0).Rows(0).Item("Colour").ToString()
            txtWidth.Text = myData.Tables(0).Rows(0).Item("Width").ToString()
            txtGroup.Text = myData.Tables(0).Rows(0).Item("Group").ToString()
            txtDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))

        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindDataFabric", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindDataDesign()
        ddlDesignId.Items.Clear()
        Try
            ddlDesignId.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Designs WHERE Active=1 ORDER BY Name ASC")
            ddlDesignId.DataTextField = "NameText"
            ddlDesignId.DataValueField = "Id"
            ddlDesignId.DataBind()

            If ddlDesignId.Items.Count > 0 Then
                ddlDesignId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindDataDesign", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)
        txtId.BackColor = Drawing.Color.Empty
        txtName.BackColor = Drawing.Color.Empty
        txtType.BackColor = Drawing.Color.Empty
        txtColour.BackColor = Drawing.Color.Empty
        txtWidth.BackColor = Drawing.Color.Empty
        txtGroup.BackColor = Drawing.Color.Empty
        ddlDesignId.BackColor = Drawing.Color.Empty
        ddlActive.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
