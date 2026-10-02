'use client';

import {
  InputGroup,
  InputGroupInput,
  InputGroupAddon,
  InputGroupButton,
} from './input-group';
import { Eye, EyeOffIcon } from 'lucide-react';
import { useState } from 'react';

export function PasswordInput({
  className,
  ...props
}: React.ComponentProps<'input'>) {
  const [showPassword, setShowPassword] = useState(false);
  return (
    <InputGroup>
      <InputGroupInput
        id={props.id ? props.id : 'password'}
        type={showPassword ? 'text' : 'password'}
        placeholder="Enter password"
        required
      />
      <InputGroupAddon align="inline-end">
        <InputGroupButton
          aria-label={showPassword ? 'Hide password' : 'Show password'}
          variant="ghost"
          size="icon-sm"
          type="button"
          onClick={() => setShowPassword(!showPassword)}
        >
          {showPassword ? (
            <Eye className="h-4 w-4" />
          ) : (
            <EyeOffIcon className="h-4 w-4" />
          )}
        </InputGroupButton>
      </InputGroupAddon>
    </InputGroup>
  );
}
