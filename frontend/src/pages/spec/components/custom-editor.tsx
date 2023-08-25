import { Highlight, themes } from "prism-react-renderer";
import Editor from "react-simple-code-editor";

import "swagger-ui-react/swagger-ui.css";

import { UseFormSetValue } from "react-hook-form";

import { cn } from "@/utils/style-utils";

import { IAddFrontendBackendSchema } from "../schemas/add-frontend-backend-schema";

interface ICustomeEditorProps {
  data: string;
  setData: UseFormSetValue<IAddFrontendBackendSchema>;
}

const CustomEditor = ({ data, setData }: ICustomeEditorProps) => {
  const highlight = (code: string) => (
    <Highlight code={code} language="yaml" theme={themes.vsDark}>
      {({ className, style, tokens, getLineProps, getTokenProps }) => (
        <div style={style} className={cn(className, "")}>
          {tokens.map((line, i) => (
            <div key={i} {...getLineProps({ line, key: i })}>
              {line.map((token, key) => (
                <span key={key} {...getTokenProps({ token, key })} />
              ))}
            </div>
          ))}
        </div>
      )}
    </Highlight>
  );

  return (
    <Editor
      value={data}
      onValueChange={(value) => setData("data", value)}
      highlight={highlight}
      padding={10}
      style={themes.vsDark.plain}
      className="min-h-[79.9vh]"
    />
  );
};

export default CustomEditor;
