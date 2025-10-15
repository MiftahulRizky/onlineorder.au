Partial Class Setting_Bottom_Add
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/bottom", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call BackColor()

            Call BindDataBracket()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            Dim bracketFinal As String = ""
            Dim bracketSelected As String = ""

            For Each row As GridViewRow In gvListBracket.Rows
                If row.RowType = DataControlRowType.DataRow Then
                    Dim chkRow As CheckBox = TryCast(row.Cells(0).FindControl("chkBracket"), CheckBox)
                    If chkRow.Checked Then
                        Dim name As String = row.Cells(0).Text

                        bracketSelected += name & ","
                    End If
                End If
            Next

            If txtId.Text = "" Then
                Call MessageError(True, "BOTTOM RAIL ID IS REQUIRED !")
                txtId.BackColor = Drawing.Color.Red
                txtId.Focus()
                Exit Sub
            End If

            If txtId.Text = publicCfg.GetItemData("SELECT Id FROM Bottoms WHERE Id='" + txtId.Text + "'") Then
                Call MessageError(True, "BOTTOM RAIL ID ALREADY EXISTS !")
                txtId.BackColor = Drawing.Color.Red
                txtId.Focus()
                Exit Sub
            End If

            If txtName.Text = "" Then
                Call MessageError(True, "BOTTOM NAME IS REQUIRED !")
                txtName.BackColor = Drawing.Color.Red
                txtName.Focus()
                Exit Sub
            End If

            If txtType.Text = "" Then
                Call MessageError(True, "BOTTOM TYPE IS REQUIRED !")
                txtType.BackColor = Drawing.Color.Red
                txtType.Focus()
                Exit Sub
            End If

            If txtColour.Text = "" Then
                Call MessageError(True, "BOTTOM COLOUR IS REQUIRED !")
                txtColour.BackColor = Drawing.Color.Red
                txtColour.Focus()
                Exit Sub
            End If

            If bracketSelected = "" Then
                Call MessageError(True, "BRACKET TYPE IS REQUIRED !")
                Exit Sub
            End If

            If ddlActive.SelectedValue = "" Then
                Call MessageError(True, "BRACKET ACTIVE IS REQUIRED !")
                ddlActive.BackColor = Drawing.Color.Red
                ddlActive.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                bracketFinal = bracketSelected.Remove(bracketSelected.Length - 1).ToString()
                lblBracketType.Text = bracketFinal
                sdsPage.Insert()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "INSERT NEW BOTTOM RAIL. ID : " & txtId.Text)

                Response.Redirect("~/setting/bottom", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/bottom", False)
    End Sub

    Private Sub BindDataBracket()
        Try
            gvListBracket.DataSource = publicCfg.GetListData("SELECT Name AS NameValue, UPPER(Name) AS NameText FROM BracketType ORDER BY Name ASC")
            gvListBracket.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        txtId.BackColor = Drawing.Color.Empty
        txtName.BackColor = Drawing.Color.Empty
        txtType.BackColor = Drawing.Color.Empty
        txtColour.BackColor = Drawing.Color.Empty
        txtDescription.BackColor = Drawing.Color.Empty
        ddlActive.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
