{{- define "dc.name" -}}digi-document-converter{{- end -}}
{{- define "dc.labels" -}}
app.kubernetes.io/name: {{ include "dc.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/part-of: docportal
{{- end -}}
