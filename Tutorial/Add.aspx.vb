Imports System.IO

Partial Class Tutorial_Add
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/tutorial", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call BackColor()

            Call BindRole()
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

            If Not fuThumbnail.HasFile Then
                Call MessageError(True, "THUMBNAIL TUTORIAL IS REQUIRED !")
                fuThumbnail.CssClass = "form-control rounded-pill is-invalid"
                fuThumbnail.Focus()
                Exit Sub
            End If

            If Not fuFile.HasFile Then
                Call MessageError(True, "FILE DOWNLOAD TUTORIAL IS REQUIRED !")
                fuFile.CssClass = "form-control rounded-pill is-invalid"
                fuFile.Focus()
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
                Dim fileNameThumbnail As String = Path.GetFileName(fuThumbnail.PostedFile.FileName.Replace(",", " - "))
                Dim pathThumbnail As String = "~/File/Tutorial/Thumbnail"
                fuThumbnail.PostedFile.SaveAs((Server.MapPath(pathThumbnail) + "/" + fileNameThumbnail))
                lblThumbnail.Text = fileNameThumbnail

                Dim fileNameFile As String = Path.GetFileName(fuFile.PostedFile.FileName.Replace(",", " - "))
                Dim pathFile As String = "~/File/Tutorial/PDF"
                fuFile.PostedFile.SaveAs((Server.MapPath(pathFile) + "/" + fileNameFile))
                lblFile.Text = fileNameFile

                roleFinal = roleSelected.Remove(roleSelected.Length - 1).ToString()
                lblRole.Text = UCase(roleFinal).ToString()

                sdsPage.Insert()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "INSERT NEW TUTORIAL. TITLE : " & txtTitle.Text)

                Response.Redirect("~/tutorial", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/tutorial", False)
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
            ' divError.Visible = True 
            Dim escapedMsg As String = HttpUtility.JavaScriptStringEncode(Msg)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showMessageError('"& escapedMsg &"')", True)
        End If
    End Sub
End Class
