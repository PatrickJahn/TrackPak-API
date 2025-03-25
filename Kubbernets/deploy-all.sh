#!/bin/bash

# Set the namespace
NAMESPACE="trackpak"

echo "Creating namespace: $NAMESPACE"
kubectl create namespace $NAMESPACE --dry-run=client -o yaml | kubectl apply -f -

# Apply all YAML files in the current directory
echo "Applying all Kubernetes manifests..."
for file in *.yaml; do
  echo "Applying $file..."
  kubectl apply -f "$file" -n $NAMESPACE
done

echo "✅ All manifests applied to namespace: $NAMESPACE"