Imports System.Data
Imports System.IO

Partial Class Tutorial_Edit
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/tutorial", False)
            Exit Sub
        End If

        If Session("tutorialDetail") = "" Then
            Response.Redirect("~/tutorial", False)
            Exit Sub
        End If

        lblId.Text = UCase(Session("tutorialDetail")).ToString()
        If Not IsPostBack Then
            Call BindDataTutorial(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            Dim roleFinal As String = ""
            Dim roleSelected As String = ""

            For Each row As GridViewRow In gvListRole.Rows
                If row.RowType = DataControlRowType.DataRow Then
                    Dim chkRow As CheckBox = TryCast(row.Cells(0).FindControl("chkRole"), CheckBox)
                    If chkRow.Checked Then
                        Dim name As String = row.Cells(0).Text

                        roleSelected += name & ","
                    End If
                End If
            Next

            If txtTitle.Text = "" Then
                Call MessageError(True, "TITLE TUTORIAL IS REQUIRED !")
                txtTitle.CssClass = "form-control rounded-pill is-invalid"
                txtTitle.Focus()
                Exit Sub
            End If

            If txtDescription.Text = "" Then
                Call MessageError(True, "DESCRIPTION TUTORIAL IS REQUIRED !")
                txtDescription.CssClass = "form-control  is-invalid"
                txtDescription.Focus()
                Exit Sub
            End If

            If roleSelected = "" Then
                Call MessageError(True, "SHOW FOR IS REQUIRED !")
                Exit Sub
            End If

            If ddlActive.SelectedValue = "" Then
                Call MessageError(True, "ACTIVE IS REQUIRED !")
                ddlActive.CssClass = "form-select rounded-pill is-invalid"
                ddlActive.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                If fuThumbnail.HasFile Then
                    Dim fileNameThumbnail As String = Path.GetFileName(fuThumbnail.PostedFile.FileName.Replace(",", " - "))
                    Dim pathThumbnail As String = "~/File/Tutorial/Thumbnail"
                    fuThumbnail.PostedFile.SaveAs((Server.MapPath(pathThumbnail) + "/" + fileNameThumbnail))
                    lblThumbnail.Text = fileNameThumbnail

                    sdsThumbnail.Update()
                End If

                If fuFile.HasFile Then
                    Dim fileNameFile As String = Path.GetFileName(fuFile.PostedFile.FileName.Replace(",", " - "))
                    Dim pathFile As String = "~/File/Tutorial/PDF"
                    fuFile.PostedFile.SaveAs((Server.MapPath(pathFile) + "/" + fileNameFile))
                    lblFile.Text = fileNameFile

                    sdsFile.Update()
                End If

                roleFinal = roleSelected.Remove(roleSelected.Length - 1).ToString()
                lblRole.Text = UCase(roleFinal).ToString()

                sdsPage.Update()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "UPDATE TUTORIAL. TITLE : " & txtTitle.Text)

                Response.Redirect("~/tutorial", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/tutorial", False)
    End Sub

    Private Sub BindDataTutorial(Id As String)
        Call BackColor()
        Try
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM Tutorials WHERE Id = '" + UCase(Id).ToString() + "'")
            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/tutorial", False)
                Exit Sub
            End If

            Call BindRole()

            txtTitle.Text = myData.Tables(0).Rows(0).Item("Title").ToString()
            txtDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
            lblThumbnail.Text = myData.Tables(0).Rows(0).Item("Thumbnail").ToString()
            lblFile.Text = myData.Tables(0).Rows(0).Item("File").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))

            Dim roleArray() As String = myData.Tables(0).Rows(0).Item("RoleId").ToString().Split(",")
            Dim roleList As List(Of String) = roleArray.ToList()

            For Each row As GridViewRow In gvListRole.Rows
                If row.RowType = DataControlRowType.DataRow Then
                    Dim chkRow As CheckBox = TryCast(row.Cells(0).FindControl("chkRole"), CheckBox)
                    For Each element As String In roleList
                        If UCase(row.Cells(0).Text).ToString() = element Then
                            chkRow.Checked = True
                        End If
                    Next
                End If
            Next
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindRole()
        Try
            gvListRole.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Roles ORDER BY Name ASC")
            gvListRole.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        txtTitle.CssClass = "form-control rounded-pill"
        txtDescription.CssClass = "form-control"
        fuThumbnail.CssClass = "form-control rounded-pill"
        fuFile.CssClass = "form-control rounded-pill"
        ddlActive.CssClass = "form-select rounded-pill"
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False 
        '  msgError.InnerText = Msg
        If Show = True Then 
        '  divError.Visible = True 
            Dim escapedMsg As String = HttpUtility.JavaScriptStringEncode(Msg)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showMessageError('"& escapedMsg &"')", True)
        End If
    End Sub
End Class
